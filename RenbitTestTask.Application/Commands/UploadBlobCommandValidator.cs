using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;

namespace RenbitTestTask.Application.Commands
{
    public class UploadBlobCommandValidator : AbstractValidator<UploadBlobCommand>
    {
        public UploadBlobCommandValidator()
        {
            RuleFor(x => x.UserEmail)
                    .NotNull()
                    .EmailAddress()
                    .WithMessage("Invalid email format.");

            RuleFor(x => x.File)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .Must(ValidateFile)
                .WithMessage("Invalid file format. Please upload .docx file");
        }

        private bool ValidateFile(IBrowserFile file)
        {
            return file.ContentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        }
    }
}
