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

            bool isFunctionCanBeTriggered = true;

            isFunctionCanBeTriggered = ValidateParams(metadata, name, storageConnectionString, containerName, isFunctionCanBeTriggered);

            if (isFunctionCanBeTriggered)
            {
                return;
            }

            // Get the file URL with SAS token
            var blobServiceClient = new BlobServiceClient(storageConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(name);

            var fileUrlWithSas = GetBlobSasToken(blobClient, sasTokenExpiryInHours);

            string userEmail = metadata["UserEmail"];

            SendEmail(userEmail, name, fileUrlWithSas);

        }

        private bool ValidateParams(IDictionary<string, string> metadata, string name, string? storageConnectionString, string? containerName, bool isFunctionCanBeTriggered)
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                isFunctionCanBeTriggered = false;
                _logger.LogError($"Function can not be executed. BREVO_API_KEY environmental variable is null or empty.");
            }
            if (string.IsNullOrEmpty(storageConnectionString))
            {
                isFunctionCanBeTriggered = false;
                _logger.LogError($"Function can not be executed. BLOB_STORAGE_CONNECTION_STRING environmental variable is null or empty.");
            }
            if (string.IsNullOrEmpty(containerName))
            {
                isFunctionCanBeTriggered = false;
                _logger.LogError($"Function can not be executed. BLOB_STORAGE_CONTAINER_NAME environmental variable is null or empty.");
            }
            if (string.IsNullOrEmpty(name))
            {
                isFunctionCanBeTriggered = false;
                _logger.LogError($"Function can not be executed. metadata is null or empty.");
            }
            if (metadata is null)
            {
                isFunctionCanBeTriggered = false;
                _logger.LogError($"Function can not be executed. metadata is null or empty.");
            }
            else
            {
                if (string.IsNullOrEmpty(metadata["UserEmail"]))
                {
                    isFunctionCanBeTriggered = false;
                    _logger.LogError($"Function can not be executed. metadata is null or empty.");
                }
            }

            return isFunctionCanBeTriggered;
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
