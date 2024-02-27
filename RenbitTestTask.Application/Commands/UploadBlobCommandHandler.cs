using MediatR;
using RenbitTestTask.Application.Services;

namespace RenbitTestTask.Application.Commands
{
    public class UploadBlobCommandHandler : IRequestHandler<UploadBlobCommand, string>
    {
        public async Task<string> Handle(UploadBlobCommand request, CancellationToken cancellationToken)
        {
            FileService service = new FileService();
            var result = new Models.BlobResponseDto();

            if (request.File is not null && !string.IsNullOrEmpty(request.UserEmail))
            {
                result = await service.UploadAsync(request.File, request.UserEmail);
            }
            else
            {
                result.Error = true;
                result.Status = "\nNull parameters. Function can not be executed";
            }

            return result.Status;
        }
    }
}
