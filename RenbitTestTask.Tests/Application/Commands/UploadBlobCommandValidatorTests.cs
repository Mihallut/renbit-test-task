using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Components.Forms;
using Xunit;

namespace RenbitTestTask.Application.Commands
{
    public class UploadBlobCommandValidatorTests
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
        public void ValidCommand_NoValidationErrors()
        {
            // Arrange
            UploadBlobCommandValidator validator = new UploadBlobCommandValidator();
            var validCommand = new UploadBlobCommand
            {
                UserEmail = "test@example.com",
                File = new MockBrowserFile("test.docx", new byte[0]) { ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document" }
            };

            var result = validator.TestValidate(validCommand);
            // Act and Assert
            result.ShouldNotHaveValidationErrorFor(x => x.UserEmail);
            result.ShouldNotHaveValidationErrorFor(x => x.File);
        }

        [Fact]
        public void InvalidEmail_FormatValidationError()
        {
            // Arrange
            var validator = new UploadBlobCommandValidator();
            var invalidCommand = new UploadBlobCommand
            {
                UserEmail = "testexample",
                File = new MockBrowserFile("test.docx", new byte[0]) { ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document" }
            };

            var result = validator.TestValidate(invalidCommand);

            // Act and Assert
            result.ShouldHaveValidationErrorFor(x => x.UserEmail)
                .WithErrorMessage("Invalid email format.");
        }

        [Fact]
        public void InvalidFile_FormatValidationError()
        {
            // Arrange
            var validator = new UploadBlobCommandValidator();
            var invalidCommand = new UploadBlobCommand
            {
                UserEmail = "test@example.com",
                File = new MockBrowserFile("TestPdf", new byte[0]) { ContentType = "application/pdf" }
            };

            var result = validator.TestValidate(invalidCommand);

            // Act and Assert
            result.ShouldHaveValidationErrorFor(x => x.File)
                .WithErrorMessage("Invalid file format. Please upload .docx file");
        }
    }
}