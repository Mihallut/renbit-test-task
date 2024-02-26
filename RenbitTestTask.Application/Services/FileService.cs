using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Components.Forms;
using RenbitTestTask.Application.Models;

namespace RenbitTestTask.Application.Services
{
    public class FileService
    {
        private readonly string _storageAccount = Environment.GetEnvironmentVariable("STORAGE_ACCOUNT_NAME");
        private readonly string _key = Environment.GetEnvironmentVariable("STORAGE_KEY");
        private readonly BlobContainerClient _filesContainer;
        private readonly BlobServiceClient _blobServiceClient;

        public FileService()
        {
            var credential = new StorageSharedKeyCredential(_storageAccount, _key);
            var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
            _blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
            _filesContainer = _blobServiceClient.GetBlobContainerClient("files");
        }

        public async Task<BlobResponseDto> UploadAsync(IBrowserFile blob, string userEmail)
        {
            BlobResponseDto response = new();
            BlobClient client = _filesContainer.GetBlobClient(blob.Name);
            IDictionary<string, string> metadata = new Dictionary<string, string>
            {
            { "UserEmail", userEmail }
            };
            try
            {
                await using (Stream data = blob.OpenReadStream())
                {
                    await client.UploadAsync(data, new BlobUploadOptions
                    {
                        HttpHeaders = new BlobHttpHeaders { ContentType = "application/octet-stream" },
                        Metadata = metadata
                    });
                }
            }
            catch (Exception ex)
            {
                response.Error = true;
                response.Status = ex.Message;
                return response;
            }
            response.Status = $"\nFile {blob.Name} uploaded sucessfuly";
            response.Error = false;
            response.Blob.Uri = client.Uri.AbsoluteUri;
            response.Blob.Name = blob.Name;

            return response;
        }
    }
}
