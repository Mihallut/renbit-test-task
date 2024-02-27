using Microsoft.AspNetCore.Components.Forms;
using Moq;
using RenbitTestTask.Application.Commands;
using RenbitTestTask.Application.Services;
using Xunit;

namespace RenbitTestTask.Tests.Application.Commands
{
    public class UploadBlobCommandHandlerTests
    {
        private class MockBrowserFile : IBrowserFile
        {
            public MockBrowserFile(string fileName, byte[] fileContent)
            {
                Name = fileName;
                content = new MemoryStream(fileContent);
                Size = fileContent.Length;
            }
            private readonly Stream content;

            public string Name { get; set; }

            public DateTimeOffset LastModified { get; set; }

            private bool reset = true;
            public long Size { get; set; }

            public string ContentType { get; set; }

            public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
            {
                if (reset)
                {
                    content.Seek(0, SeekOrigin.Begin);
                }
                return content;
            }
        }

        [Fact]
        public async Task Handle_ValidRequest_ReturnsStatus()
        {

            // Arrange
            var request = new UploadBlobCommand
            {
                File = new MockBrowserFile("TestFile", new byte[0]) { ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                UserEmail = "test@example.com"
            };

            var cancellationToken = new CancellationToken();

            var fileServiceMock = new Mock<FileService>();
            var expectedResult = new RenbitTestTask.Application.Models.BlobResponseDto
            {
                Status = $"\nFile {request.File.Name} uploaded sucessfuly"
            };

            var handler = new UploadBlobCommandHandler();

            // Act
            var result = await handler.Handle(request, cancellationToken);

            // Assert
            Assert.Equal(expectedResult.Status, result);
        }

        [Fact]
        public async Task Handle_NullFile_ReturnsErrorMessage()
        {
            // Arrange
            var request = new UploadBlobCommand
            {
                File = null,
                UserEmail = "test@example.com"
            };

            var cancellationToken = new CancellationToken();

            var handler = new UploadBlobCommandHandler();

            // Act
            var result = await handler.Handle(request, cancellationToken);

            // Assert
            Assert.Contains("Null parameters. Function can not be executed", result);
        }

        [Fact]
        public async Task Handle_NullEmail_ReturnsErrorMessage()
        {
            // Arrange
            var request = new UploadBlobCommand
            {
                File = new MockBrowserFile("TestFile", new byte[0]) { ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                UserEmail = null
            };

            var cancellationToken = new CancellationToken();

            var handler = new UploadBlobCommandHandler();

            // Act
            var result = await handler.Handle(request, cancellationToken);

            // Assert
            Assert.Contains("Null parameters. Function can not be executed", result);
        }
    }
}
