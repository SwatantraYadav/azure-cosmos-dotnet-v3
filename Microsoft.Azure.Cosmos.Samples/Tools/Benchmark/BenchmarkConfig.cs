﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace CosmosBenchmark
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime;
    using CommandLine;

    public class BenchmarkConfig
    {
        [Option('w', Required = true, HelpText = "Workload type insert, read")]
        public string WorkloadType { get; set; }

        [Option('e', Required = true, HelpText = "Cosmos account end point")]
        public string EndPoint { get; set; }

        [Option('k', Required = true, HelpText = "Cosmos account master key")]
        public string Key { get; set; }

        [Option(Required = false, HelpText = "Database to use")]
        public string Database { get; set; } = "db";

        [Option(Required = false, HelpText = "Collection to use")]
        public string Container { get; set; } = "data";

        [Option('t', Required = false, HelpText = "Collection throughput use")]
        public int Throughput { get; set; } = 100000;

        [Option('n', Required = false, HelpText = "Number of documents to insert")]
        public int ItemCount { get; set; } = 200000;

        [Option(Required = false, HelpText = "Start with new collection")]
        public string ConsistencyLevel { get; set; }

        [Option(Required = false, HelpText = "Start with new collection")]
        public bool CleanupOnStart { get; set; } = false;

        [Option(Required = false, HelpText = "Clean-up after run")]
        public bool CleanupOnFinish { get; set; } = false;

        [Option(Required = false, HelpText = "Container partition key path")]
        public string PartitionKeyPath { get; set; } = "/partitionKey";

        [Option("pl", Required = false, HelpText = "Degree of parallism")]
        public int DegreeOfParallelism { get; set; } = -1;

        [Option(Required = false, HelpText = "Item template")]
        public string ItemTemplateFile { get; set; } = "Player.json";

        [Option(Required = false, HelpText = "Min thread pool size")]
        public int MinThreadPoolSize { get; set; } = 100;

        internal int GetTaskCount(int containerThroughput)
        {
            int taskCount = this.DegreeOfParallelism;
            if (taskCount == -1)
            {
                // set TaskCount = 10 for each 10k RUs, minimum 1, maximum { #processor * 50 }
                taskCount = Math.Max(containerThroughput / 1000, 1);
                taskCount = Math.Min(taskCount, Environment.ProcessorCount * 50);
            }

            return taskCount;
        }

        internal void Print()
        {
            using (ConsoleColorContext ct = new ConsoleColorContext(ConsoleColor.Green))
            {
                Console.WriteLine($"{nameof(BenchmarkConfig)} arguments");
                Console.WriteLine($"IsServerGC: {GCSettings.IsServerGC}");
                Console.WriteLine("--------------------------------------------------------------------- ");
                Console.WriteLine(JsonHelper.ToString(this));
                Console.WriteLine("--------------------------------------------------------------------- ");
                Console.WriteLine();
            }
        }

        internal static BenchmarkConfig From(string[] args)
        {
            BenchmarkConfig options = null;
            Parser.Default.ParseArguments<BenchmarkConfig>(args)
                .WithParsed<BenchmarkConfig>(e => options = e)
                .WithNotParsed<BenchmarkConfig>(e => BenchmarkConfig.HandleParseError(e));

            return options;
        }

        private static void HandleParseError(IEnumerable<Error> errors)
        {
            using (ConsoleColorContext ct = new ConsoleColorContext(ConsoleColor.Red))
            {
                foreach (Error e in errors)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            Environment.Exit(errors.Count());
        }
    }
}
