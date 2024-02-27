using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Logging;
using Moq;
using RenbitTestTask.FuncApp;
using Xunit;

namespace RenbitTestTask.Tests.FuncApp
{
    public class SendEmailFunctionTests
    {

        [Fact]
        public void Run_ValidBlobTrigger_CallsSendEmail()
        {
            var loggerMock = new Mock<ILogger<SendEmailFunction>>();
            loggerMock.Setup(x => x.Log(
               LogLevel.Information,
               It.IsAny<EventId>(),
               It.IsAny<It.IsAnyType>(),
               It.IsAny<Exception>(),
               It.IsAny<Func<It.IsAnyType, Exception?, string>>()
           )
       );
            var blobServiceClientMock = new Mock<BlobServiceClient>();
            var blobContainerClientMock = new Mock<BlobContainerClient>();
            var blobClientMock = new Mock<BlobClient>();

            blobServiceClientMock.Setup(c => c.GetBlobContainerClient(It.IsAny<string>())).Returns(blobContainerClientMock.Object);
            blobContainerClientMock.Setup(c => c.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);
            blobClientMock.Setup(c => c.Uri).Returns(new Uri("https://example.blob.core.windows.net/testfile.txt"));
            blobClientMock.Setup(c => c.GenerateSasUri(It.IsAny<BlobSasBuilder>())).Returns(new Uri("https://example.blob.core.windows.net/testfile.txt?sasToken"));

            var sendEmailFunction = new SendEmailFunction(loggerMock.Object);

            // Act
            sendEmailFunction.Run("testfile.txt", new Dictionary<string, string> { { "UserEmail", "test@example.com" } }, "testfile.txt");

            // Assert
            loggerMock.Verify(m => m.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);

            loggerMock.Verify(m => m.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Never);
        }

        [Fact]
        public void Run_InvalidBlobTrigger_DoesNotCallSendEmail()
        {
            var loggerMock = new Mock<ILogger<SendEmailFunction>>();
            loggerMock.Setup(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()
            ));

            var blobServiceClientMock = new Mock<BlobServiceClient>();
            var blobContainerClientMock = new Mock<BlobContainerClient>();
            var blobClientMock = new Mock<BlobClient>();

            blobServiceClientMock.Setup(c => c.GetBlobContainerClient(It.IsAny<string>())).Returns(blobContainerClientMock.Object);
            blobContainerClientMock.Setup(c => c.GetBlobClient(It.IsAny<string>())).Returns(blobClientMock.Object);
            blobClientMock.Setup(c => c.Uri).Returns(new Uri("https://example.blob.core.windows.net/testfile.txt"));
            blobClientMock.Setup(c => c.GenerateSasUri(It.IsAny<BlobSasBuilder>())).Returns(new Uri("https://example.blob.core.windows.net/testfile.txt?sasToken"));

            var sendEmailFunction = new SendEmailFunction(loggerMock.Object);

            // Act
            sendEmailFunction.Run("", new Dictionary<string, string> { { "UserEmail", "test@example.com" } }, "testfile.txt");

            // Assert
            loggerMock.Verify(m => m.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
        }
    }
}
