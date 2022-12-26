﻿using CommandLine;
using Kafka.Cli.Text;
using Kafka.Cli.Verbs;
using Kafka.Client.Clients.Admin;
using Kafka.Client.Clients.Admin.Model;

namespace Kafka.Cli.Cmd
{
    internal static class Topics
    {
        public static async ValueTask<int> Parse(
            IEnumerable<string> args,
            CancellationToken cancellationToken
        ) =>
            await Parser.Default.ParseArguments<VerbTopicsList, VerbTopicCreate, VerbTopicDescribe, VerbTopicDelete>(args)
                .MapResult(
                    (VerbTopicsList verb) => List(verb, cancellationToken),
                    (VerbTopicCreate verb) => Create(verb, cancellationToken),
                    (VerbTopicDescribe verb) => Describe(verb, cancellationToken),
                    (VerbTopicDelete verb) => Delete(verb, cancellationToken),
                    errs => new ValueTask<int>(-1)
                )
            ;

        public static async ValueTask<int> List(
            VerbTopicsList verb,
            CancellationToken cancellationToken
        )
        {
            try
            {
                var adminClientConfig = new AdminClientConfig
                {
                    BootstrapServers = verb.BootstrapServer
                };
                using var adminClient = (IAdminClient)new Client.Clients.Admin.AdminClient(adminClientConfig);
                var options = new ListTopicsOptionsBuilder(adminClientConfig)
                    .IncludeInternal(!verb.ExcludeInternal)
                    .Build()
                ;
                var result = await adminClient.ListTopics(
                    options,
                    cancellationToken
                );
                foreach (var topic in result.Topics)
                    Console.WriteLine(topic.Name);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return -1;
            }
        }

        public static async ValueTask<int> Create(
            VerbTopicCreate verb,
            CancellationToken cancellationToken
        )
        {
            try
            {
                var adminClientConfig = new AdminClientConfig
                {
                    BootstrapServers = verb.BootstrapServer
                };
                using var adminClient = (IAdminClient)new Client.Clients.Admin.AdminClient(adminClientConfig);
                var replicaAssinment = new Dictionary<int, int[]>();
                var partitionReplicaAssignments = verb.ReplicaAssignment.Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (var partitionReplicaAssignment in partitionReplicaAssignments)
                {
                    var kv = partitionReplicaAssignment.Split('=', StringSplitOptions.RemoveEmptyEntries);
                    var key = int.Parse(kv[0]);
                    var value = kv[1].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(r => int.Parse(r)).ToArray();
                    replicaAssinment.Add(key, value);
                }
                var options = new CreateTopicsOptionsBuilder(adminClientConfig)
                    .NewTopic(b => b
                        .Name(verb.Topic)
                        .NumPartitions(verb.PartitionCount)
                        .ReplicasAssignments(replicaAssinment)
                        .Build()
                    )
                    .Build()
                ;
                var result = await adminClient.CreateTopics(
                    options,
                    cancellationToken
                );
                if (result.CreatedTopics.Any())
                {
                    Console.WriteLine("Created topics:");
                    foreach (var topic in result.CreatedTopics)
                    {
                        Console.Write($"  {topic.Name}");
                        if(topic.Id != Guid.Empty)
                            Console.Write($" ({topic.Id})");
                        Console.WriteLine();
                        Console.WriteLine($"    Partitions:         {topic.NumPartitions}");
                        Console.WriteLine($"    Replication Factor: {topic.ReplicationFactor}");
                        Console.WriteLine($"    Configs:");
                        foreach(var kv in topic.Config)
                            Console.WriteLine($"      {kv.Key}: {kv.Value}");
                    }
                }
                if (result.ErrorTopics.Any())
                {
                    Console.WriteLine("Failed topics:");
                    foreach (var topic in result.ErrorTopics)
                    {
                        Console.Write($"  {topic.Name}");
                        Console.WriteLine();
                        Console.WriteLine($"    {Formatter.Print(topic.Error)}");
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return -1;
            }
        }

        public static async ValueTask<int> Delete(
            VerbTopicDelete verb,
            CancellationToken cancellationToken
        )
        {
            try
            {
                var adminClientConfig = new AdminClientConfig
                {
                    BootstrapServers = verb.BootstrapServer
                };
                using var adminClient = (IAdminClient)new Client.Clients.Admin.AdminClient(adminClientConfig);
                var options = new DeleteTopicsOptionsBuilder(adminClientConfig)
                    .TopicName(verb.TopicName)
                    .TopicId(verb.TopicId)
                    .Build()
                ;
                var result = await adminClient.DeleteTopics(
                    options,
                    cancellationToken
                );
                foreach (var topic in result.DeletedTopics)
                    Console.WriteLine(topic);
                foreach (var error in result.ErrorTopics)
                    Console.WriteLine(error);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return -1;
            }
        }

        public static async ValueTask<int> Describe(
            VerbTopicDescribe verb,
            CancellationToken cancellationToken
        )
        {
            try
            {
                var adminClientConfig = new AdminClientConfig
                {
                    BootstrapServers = verb.BootstrapServer
                };
                using var adminClient = (IAdminClient)new Client.Clients.Admin.AdminClient(adminClientConfig);
                var options = new DescribeTopicsOptionsBuilder(adminClientConfig)
                    .TopicName(verb.TopicName)
                    .TopicId(verb.TopicId)
                    .Build()
                ;
                var result = await adminClient.DescribeTopics(
                    options,
                    cancellationToken
                );
                foreach (var topic in result.Topics.Values)
                {
                    Console.WriteLine($"Id: {topic.TopicId}");
                    Console.WriteLine($"Name: {topic.Name}");
                    Console.WriteLine($"Internal: {topic.IsInternal}");
                    Console.WriteLine($"AuthorizedOperations: {topic.TopicAuthorizedOperations}");
                    Console.WriteLine($"Error: {Formatter.Print(topic.Error)}");
                    foreach (var partition in topic.Partitions.OrderBy(r => r.PartitionIndex))
                    {
                        Console.WriteLine($"Partition: {partition.PartitionIndex}");
                        Console.WriteLine($"  LeaderId: {partition.LeaderId}");
                        Console.WriteLine($"  LeaderEpoch: {partition.LeaderEpoch}");
                        Console.WriteLine($"  Error: {Formatter.Print(topic.Error)}");
                        Console.WriteLine($"  ReplicaNodes: [{string.Join(',', partition.ReplicaNodes)}]");
                        Console.WriteLine($"  IsrNodes: [{string.Join(',', partition.IsrNodes)}]");
                        Console.WriteLine($"  OfflineReplicas: [{string.Join(',', partition.OfflineReplicas)}]");
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return -1;
            }
        }
    }
}
