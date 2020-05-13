using System;
using System.Collections.Generic;

namespace Azure.Storage.Abstractions
{
    public class StorageSettings
    {
        public StorageSettings()
        {

        }
        public StorageSettings(string connectionString, string containerName) : this(Guid.NewGuid().ToString(), connectionString, containerName, new TimeSpan(0,1,0), new TimeSpan(0, 0, 10), true,1)
        {

        }
        
        public StorageSettings(string clientId, string connectionString, string containerName, TimeSpan serverTimeout, TimeSpan executionRetriesTimeout, bool integrityValidation, int? parallelBlocksUploaded = null)
        {
            this.ConnectionString = connectionString;
            this.ContainerName = containerName;
            this.ServerTimeout = serverTimeout;
            this.ExecutionRetriesTimeout = executionRetriesTimeout;
            this.IntegrityValidation = integrityValidation;
            this.ParallelBlocksUploaded = parallelBlocksUploaded;
            this.ClientId = clientId;
        }
        
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }

        public TimeSpan ServerTimeout { get; set; }
        public TimeSpan ExecutionRetriesTimeout { get; set; }
        public bool IntegrityValidation { get; set; }
        public int? ParallelBlocksUploaded { get; set; }
        public bool? EncryptReadAndWrite { get; set; }
        public TimeSpan RetryBackoff { get; set; }
        public int RetryAttempts { get; set; }
        public string ClientId { get; set; }
        public IDictionary<string, string> OperationHeaders { get; internal set; }
    }
}