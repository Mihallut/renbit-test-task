using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace RenbitTestTask.FuncApp
{
    public class SendEmailFunction
    {
        private readonly ILogger _logger;

        public SendEmailFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<SendEmailFunction>();
        }

        [Function("SendEmailFunction")]
        public void Run([BlobTrigger("files/{name}", Connection = "")] string myBlob, string name)
        {
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n");
        }
    }
}
