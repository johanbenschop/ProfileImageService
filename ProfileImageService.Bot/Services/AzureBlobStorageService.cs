using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using ProfileImageService.Features.PhotoHandler.Models;

namespace ProfileImageService.Bot.Services
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
            await SaveStreamToBlob(processedFace.PhotoOfFaceWithoutBackgroundStream, $"{processedFace.Face.FaceId}.png", cancellationToken);
            return await SaveStreamToBlob(processedFace.ProfileImageStream, $"{processedFace.Face.FaceId}.png", cancellationToken);
        }

        private async Task<string> SaveStreamToBlob(Stream stream, string fileName, CancellationToken cancellationToken)
        {
            var cloudBlockBlob = _blobContainer.GetBlockBlobReference(fileName);
            await cloudBlockBlob.UploadFromStreamAsync(stream, cancellationToken);

            return cloudBlockBlob.Uri.AbsoluteUri;
        }
    }
}
