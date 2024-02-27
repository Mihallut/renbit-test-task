using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;

namespace RenbitTestTask.FuncApp
{
    public class SendEmailFunction
    {
        private readonly ILogger _logger;
        private readonly string _apiKey = Environment.GetEnvironmentVariable("BREVO_API_KEY");

        public SendEmailFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SendEmailFunction>();
        }

        [Function("SendEmailFunction")]
        public void Run([BlobTrigger("files/{name}", Connection = "AzureWebJobsStorage")]
        string blob,
        IDictionary<string, string> metadata, string name)
        {
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n");
            var storageConnectionString = Environment.GetEnvironmentVariable("BLOB_STORAGE_CONNECTION_STRING");

            // Get the container name and SAS token expiry duration from Application Settings
            var containerName = Environment.GetEnvironmentVariable("BLOB_STORAGE_CONTAINER_NAME");
            var sasTokenExpiryInHours = 1;

            // Get the file URL with SAS token
            var blobServiceClient = new BlobServiceClient(storageConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(name);

            var fileUrlWithSas = GetBlobSasToken(blobClient, sasTokenExpiryInHours);

            string userEmail = metadata["UserEmail"];

            SendEmail(userEmail, name, fileUrlWithSas);

        }

        private void SendEmail(string userEmail, string blobName, string sasLink)
        {
            Configuration.Default.ApiKey["api-key"] = _apiKey;

            var apiInstance = new TransactionalEmailsApi();

            string SenderName = "Reenbit Test Task Sender";
            string SenderEmail = "reenbittesttask@sender.com";
            SendSmtpEmailSender Email = new SendSmtpEmailSender(SenderName, SenderEmail);

            SendSmtpEmailTo smtpEmailTo = new SendSmtpEmailTo(userEmail);
            List<SendSmtpEmailTo> To = new List<SendSmtpEmailTo>();
            To.Add(smtpEmailTo);

            string HtmlContent = null;
            string TextContent = $"Your file {blobName} have been successfuly uploaded to blob storage \n Here is link with SAS than will be availible 1 hour: \n {sasLink}";
            string Subject = "File uploaded to blob storage";

            try
            {
                var sendSmtpEmail = new SendSmtpEmail(Email, To, null, null, HtmlContent, TextContent, Subject);
                CreateSmtpEmail result = apiInstance.SendTransacEmail(sendSmtpEmail);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error occured while sending the email to {userEmail}");
            }
        }

        private static string GetBlobSasToken(BlobClient blobClient, int expiryInHours)
        {
            var blobSasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = blobClient.BlobContainerName,
                BlobName = blobClient.Name,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(expiryInHours),
                Protocol = SasProtocol.Https
            };

            blobSasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blobClient.GenerateSasUri(blobSasBuilder).ToString();
        }
    }
}
