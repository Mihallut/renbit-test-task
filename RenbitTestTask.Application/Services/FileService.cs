using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Components.Forms;
using RenbitTestTask.Application.Models;

namespace RenbitTestTask.Application.Services
{
    public class FileService
    {
        private readonly string _storageAccount = "blobcontainermihallut";
        private readonly string _key = "fCMwO+UDnn4mOEshiU6+jNS6LjB2OXkwfCEdV4FOja+Ts513VkVCHJzrtu1LNKBLkSSU3RaWBoaF+AStNndS/w==";
        private readonly BlobContainerClient _filesContainer;

        public FileService()
        {
            var credential = new StorageSharedKeyCredential(_storageAccount, _key);
            var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
            _filesContainer = new BlobContainerClient(new Uri(blobUri), credential);
        }

        public async Task<BlobResponseDto> UploadAsync(IBrowserFile blob)
        {
            BlobResponseDto response = new();
            BlobClient client = _filesContainer.GetBlobClient(blob.Name);

            await using (Stream data = blob.OpenReadStream())
            {
                await client.UploadAsync(data);
            }

            response.Status = $"File {blob.Name} uploaded sucessfuly";
            response.Error = false;
            response.Blob.Uri = client.Uri.AbsoluteUri;
            response.Blob.Name = blob.Name;

            return response;
        }
    }
}
