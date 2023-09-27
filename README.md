# Introduction

The goal of this project is to create a native .Net Client library and commandline utility for Kafka.
> Disclaimer: This is early days so do expect a bit of randomness ...

## Getting Started

This project required [.Net 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) or higher on your machine.

To run the Cli:

- Clone the repository and run `dotnet build`
- This should output a `kafka-cli` executable in directory `./src/Kafka.Cli/bin/Debug/net7.0`
- Run the `kafka-cli` executable

Creating a topic:

```bash
./kafka-cli.exe topics create --bootstrap-server localhost:9092 --topic test-topic --partition-count 6 --replication-factor 3
```

Producing to a topic:

```bash
./kafka-cli.exe producer --bootstrap-server localhost:9092 --topic test-topic
```

The produce command only accepts comma separated key value pairs eg. "some_key,some_value".

Consuming from a topic:

```bash
./kafka-cli.exe consumer --bootstrap-server localhost:9092 --topic test-topic --group-id test-cg
```
