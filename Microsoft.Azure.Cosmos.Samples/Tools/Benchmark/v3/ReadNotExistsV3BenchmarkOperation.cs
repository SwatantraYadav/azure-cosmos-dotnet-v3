﻿//------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//------------------------------------------------------------

namespace CosmosBenchmark
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;

    internal class ReadNotExistsV3BenchmarkOperation : IBenchmarkOperatrion
    {
        private readonly Container container;

        private readonly string databsaeName;
        private readonly string containerName;

        private string nextExecutionItemPartitionKey;
        private string nextExecutionItemId;

        public ReadNotExistsV3BenchmarkOperation(
            Container container)
        {
            this.container = container;

            this.databsaeName = container.Database.Id;
            this.containerName = container.Id;
        }

        public async Task<OperationResult> ExecuteOnceAsync()
        {
            using (ResponseMessage itemResponse = await this.container.ReadItemStreamAsync(
                        this.nextExecutionItemId,
                        new Microsoft.Azure.Cosmos.PartitionKey(this.nextExecutionItemPartitionKey)))
            {
                if (itemResponse.StatusCode != HttpStatusCode.NotFound)
                {
                    throw new Exception($"ReadItem failed wth {itemResponse.StatusCode}");
                }

                double ruCharges = itemResponse.Headers.RequestCharge;
                return new OperationResult()
                {
                    DatabseName = databsaeName,
                    ContainerName = containerName,
                    RuCharges = ruCharges,
                    lazyDiagnostics = () => itemResponse.Diagnostics.ToString(),
                };
            }
        }

        public Task Prepare()
        {
            if (string.IsNullOrEmpty(this.nextExecutionItemId) ||
                string.IsNullOrEmpty(this.nextExecutionItemPartitionKey))
            {
                this.nextExecutionItemId = Guid.NewGuid().ToString();
                this.nextExecutionItemPartitionKey = Guid.NewGuid().ToString();
            }

            return Task.CompletedTask;
        }
    }
}
