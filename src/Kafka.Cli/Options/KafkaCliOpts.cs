﻿using CommandLine;
using Microsoft.Extensions.Logging;

namespace Kafka.Cli.Options
{
    public abstract class KafkaCliOpts
    {
        [Option("bootstrap-server", Required = true)]
        public string BootstrapServer { get; set; } = "";

        [Option("log-level", Required = false, Default = LogLevel.Warning)]
        public LogLevel LogLevel { get; set; } = LogLevel.Warning;

        [Option("property")]
        public IEnumerable<string> Properties { get; set; } = Array.Empty<string>();
    }
}
