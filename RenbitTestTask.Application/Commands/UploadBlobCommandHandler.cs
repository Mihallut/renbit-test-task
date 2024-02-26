using MediatR;
using RenbitTestTask.Application.Services;

namespace RenbitTestTask.Application.Commands
{
    public class UploadBlobCommandHandler : IRequestHandler<UploadBlobCommand, string>
    {
        public async Task<string> Handle(UploadBlobCommand request, CancellationToken cancellationToken)
        {
            FileService service = new FileService();
            var result = await service.UploadAsync(request.File);

            return result.Status;
        }
    }
}
