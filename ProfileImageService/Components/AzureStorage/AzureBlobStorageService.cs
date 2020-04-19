using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using ProfileImageService.Components.PhotoProcessor.Models;

namespace ProfileImageService.Components.AzureStorage
{
    public class AzureBlobStorageService
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly CloudBlobContainer _blobContainer;

        public AzureBlobStorageService(CloudStorageAccount storageAccount)
        {
            _storageAccount = storageAccount;
            var cloudBlobClient = _storageAccount.CreateCloudBlobClient();
            _blobContainer = cloudBlobClient.GetContainerReference("profilepictures");
        }

        public async Task<string> SaveProcessedFaceAsync(ProcessedFace processedFace, CancellationToken cancellationToken = default)
        {
            await SaveMemoryToBlob(processedFace.TransparentPhoto, $"{processedFace.Face.FaceId}_raw.png", cancellationToken);
            return await SaveMemoryToBlob(processedFace.ProfileImage, $"{processedFace.Face.FaceId}.png", cancellationToken);
        }

        private async Task<string> SaveMemoryToBlob(ReadOnlyMemory<byte> memory, string fileName, CancellationToken cancellationToken)
        {
            var cloudBlockBlob = _blobContainer.GetBlockBlobReference(fileName);
            await cloudBlockBlob.UploadFromByteArrayAsync(memory.ToArray(), 0, memory.Length, cancellationToken);

            return cloudBlockBlob.Uri.AbsoluteUri;
        }
    }
}
