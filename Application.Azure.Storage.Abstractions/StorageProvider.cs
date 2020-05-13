using Microsoft.Azure;
using Microsoft.Azure.KeyVault.Core;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.Storage.Abstractions
{
    public abstract class StorageProvider
    {
        protected readonly StorageSettings settings;
        private CloudStorageAccount storageAccount;
        private CloudBlobClient blobClient;
        
        private CloudBlobContainer container;
        protected StorageProvider(StorageSettings settings)
        {
            this.settings = settings;
        }

        protected void IsCancelled(CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
                throw new OperationCanceledException();
        }

        private CloudStorageAccount GetCloudStorageAccount()
        {
            if(storageAccount == null)
                storageAccount = CloudStorageAccount.Parse(this.settings.ConnectionString);
            return storageAccount;
        }


        protected virtual async Task<CloudBlobContainer> GetOrCreateContainer()
        {

            var _storageAccount = GetCloudStorageAccount();
            CloudBlobClient _blobClient = this.GetOrCreateBlobClient();

            var requestOptions = CreateBlobRequestOptions(this.settings);
            var operationContext = CreateOperationContext();

            container = _blobClient.GetContainerReference(this.settings.ContainerName);
            await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, requestOptions, operationContext);
            return container;
        }

        protected virtual CloudBlobDirectory GetVirtualDirectory(string location) 
        {
            var directory = container.GetDirectoryReference(location);
            return directory;
            //CloudBlockBlob blob = directory.GetBlockBlobReference(fileName);
        }

        protected virtual CloudBlockBlob GetBlobInVirtualDirectory(string location, string fileName)
        {
            var dir = GetVirtualDirectory(location);
            CloudBlockBlob blob = dir.GetBlockBlobReference(fileName);
            return blob;
        }

        protected virtual OperationContext CreateOperationContext() 
        {
            return new OperationContext() 
            {
                 ClientRequestID = this.settings.ClientId,
                 UserHeaders = settings.OperationHeaders,
                 LogLevel = LogLevel.Informational,
            };
        }
        protected virtual BlobRequestOptions CreateBlobRequestOptions(StorageSettings _settings = null)
        {
            if (_settings == null)
                _settings = this.settings;

                return new BlobRequestOptions()
            {
                ServerTimeout = _settings.ServerTimeout,
                AbsorbConditionalErrorsOnRetry = false, //Conditional probability of failure is the probability that a specific item, such as a piece of equipment, material or system fails at a certain time interval. This is with the condition that the item has not yet failed at the current time
                DisableContentMD5Validation = !_settings.IntegrityValidation,
                LocationMode = LocationMode.PrimaryThenSecondary,
                MaximumExecutionTime = _settings.ExecutionRetriesTimeout,
                ParallelOperationThreadCount = _settings.ParallelBlocksUploaded,
                RequireEncryption = _settings.EncryptReadAndWrite,
                StoreBlobContentMD5 = _settings.IntegrityValidation,
                UseTransactionalMD5 = _settings.IntegrityValidation,
                RetryPolicy = new ExponentialRetry(_settings.RetryBackoff, _settings.RetryAttempts)
            };
        }

        protected virtual CloudBlobClient GetOrCreateBlobClient() {

            var account = GetCloudStorageAccount();
            if (blobClient == null)
                blobClient = account.CreateCloudBlobClient();
            return blobClient;
        }

        




    }
}