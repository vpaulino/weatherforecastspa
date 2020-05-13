using Azure.Storage.Abstractions;
using Azure.Storage.Abstractions.Blobs;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Azure.Storage.Blobs
{
    public class BlobStorageProvider : StorageProvider, IBlobStorageProvider
    {
       
        public BlobStorageProvider(StorageSettings settings) : base(settings)
        {
            
        }

       

        /// <summary>
        /// Lists all containers that exists on the account
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public virtual async Task<ListResult<ContainerItem>> ListContainers(CancellationToken ct)
        {
            
            var client = GetOrCreateBlobClient();
            BlobContinuationToken blobContinuationToken = null;
            List<CloudBlobContainer> cloudBlobContainers = new List<CloudBlobContainer>();

            do
            {
                var result = await client.ListContainersSegmentedAsync(blobContinuationToken, ct);
                blobContinuationToken = result.ContinuationToken;
                cloudBlobContainers.AddRange(result.Results);
            } while (blobContinuationToken != null);
            
            return new ListResult<ContainerItem>(cloudBlobContainers.Select((container) => new ContainerItem(container.Uri, container.Name)));
        }

        /// <summary>
        /// Renames the blob to a new name
        /// </summary>
        /// <param name="location">container name</param>
        /// <param name="currentFileName">current file name to be changed</param>
        /// <param name="newFileName">new file name</param>
        /// <param name="ct">operation cancellation token</param>
        /// <returns></returns>
        public virtual async Task RenameBlob(string location, string currentFileName, string newFileName, CancellationToken ct)
        {
            var blobClient = GetOrCreateBlobClient();
             
            CloudBlockBlob blobCopy = GetBlobInVirtualDirectory(location, newFileName);

            if (!await blobCopy.ExistsAsync())
            {
                CloudBlockBlob blob = GetBlobInVirtualDirectory(location, currentFileName);
                
                if (await blob.ExistsAsync(ct))
                {
                    await blobCopy.StartCopyAsync(blob, ct);
                    await blob.DeleteIfExistsAsync();
                }
            }
        }

        /// <summary>
        /// Moves one blob from one location to other location
        /// </summary>
        /// <param name="currentlocation">current container name</param>
        /// <param name="newLocation">destination container name</param>
        /// <param name="fileName">file to move</param>
        /// <param name="ct">operation container name</param>
        /// <returns></returns>
        public virtual async Task MoveBlob(string currentlocation, string newLocation, string fileName, CancellationToken ct)
        {
             
            CloudBlobClient blobClient = this.GetOrCreateBlobClient();

           var blobMoved = this.GetBlobInVirtualDirectory(newLocation, fileName);

            if (!await blobMoved.ExistsAsync())
            {

                var blobToMove  = this.GetBlobInVirtualDirectory(newLocation, fileName);
                if (await blobToMove.ExistsAsync(ct))
                {
                    await blobMoved.StartCopyAsync(blobToMove, ct);
                    await blobToMove.DeleteIfExistsAsync();
                }
            }
        }

        /// <summary>
        /// Lists all blobs that exists on the location
        /// </summary>
        /// <param name="location">container name</param>
        /// <param name="ct">operation cancellation token</param>
        /// <returns></returns>
        public virtual async Task<ListResult<BlobDescription>> ListBlobs(string location,  CancellationToken ct)
        {
           
            BlobContinuationToken blobContinuationToken = null;

            List<IListBlobItem> listResult = new List<IListBlobItem>();

            var directory = this.GetVirtualDirectory(location);

            do
            {
                var result = await directory.ListBlobsSegmentedAsync(blobContinuationToken, ct);
                blobContinuationToken = result.ContinuationToken;
                listResult.AddRange(result.Results);
            } while (blobContinuationToken != null);

            return new ListResult<BlobDescription>(listResult.Select((listBlobItem)=> new BlobDescription(listBlobItem.Uri, listBlobItem.Container.Name, listBlobItem is CloudBlobDirectory)));
        }
         
        /// <summary>
        /// Gets all the detailed information regarding an existing blob
        /// </summary>
        /// <param name="location">container name</param>
        /// <param name="name">file name</param>
        /// <param name="ct">operation cancellation token</param>
        /// <returns></returns>
        public virtual async Task<BlobDescriptionDetails> GetBlobDetails(string location, string name, CancellationToken ct)
        {
           var blob = this.GetBlobInVirtualDirectory(location, name);
            return blob.ConvertToBlobDescriptionDetails();

        }

        /// <summary>
        /// Deletes blob from the blob storage
        /// </summary>
        /// <param name="location">blob container name</param>
        /// <param name="fileName">file name</param>
        /// <param name="ct">operation cancellation token</param>
        /// <returns></returns>
        public virtual async Task<BlobDescriptionDetails> DeleteBlob(string location, string fileName, CancellationToken ct)
        {
            var blockBlob = this.GetBlobInVirtualDirectory(location, fileName);
            var result = await blockBlob.DeleteIfExistsAsync(ct);
            return blockBlob.ConvertToBlobDescriptionDetails();
        }

        /// <summary>
        /// Downloads blob from the cloud storage
        /// </summary>
        /// <param name="location">container name</param>
        /// <param name="fileName">file name to download</param>
        /// <param name="destination">stream destination</param>
        /// <param name="ct">operation cancellation token</param>
        /// <returns></returns>
        public virtual async Task<BlobDescriptionDetails> Download(string location, string fileName, Stream destination, CancellationToken ct)
        {

            var blockBlob = this.GetBlobInVirtualDirectory(location, fileName);
            await blockBlob.DownloadToStreamAsync(destination, ct);
            return blockBlob.ConvertToBlobDescriptionDetails();
        }

        /// <summary>
        /// Uploads stream content as a blob 
        /// </summary>
        /// <param name="stream">content to be uploaded</param>
        /// <param name="location">container name</param>
        /// <param name="fileName">blob name to be set</param>
        /// <param name="ct">operation cancellation token</param>
        /// <returns></returns>
        public virtual async Task<BlobDescriptionDetails> Upload(Stream stream, string location, string fileName, string contentType, CancellationToken ct)
        {
           var cloudBlob = this.GetBlobInVirtualDirectory(location, fileName);

            cloudBlob.Properties.ContentType = contentType;


            await cloudBlob.UploadFromStreamAsync(stream,new AccessCondition(), CreateBlobRequestOptions(),CreateOperationContext(), ct);

            
            return cloudBlob.ConvertToBlobDescriptionDetails();
        }
    }
}
