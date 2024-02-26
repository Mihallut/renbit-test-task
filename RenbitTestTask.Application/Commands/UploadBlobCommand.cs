using MediatR;
using Microsoft.AspNetCore.Components.Forms;

namespace RenbitTestTask.Application.Commands
{
    public class UploadBlobCommand : IRequest<string>
    {
        public string UserEmail { get; set; }
        public IBrowserFile File { get; set; }
    }
}
