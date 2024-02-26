namespace RenbitTestTask.Application.Models
{
    public class BlobResponseDto
    {
        public BlobDto Blob { get; set; }
        public bool Error { get; set; }
        public string Status { get; set; }

        public BlobResponseDto()
        {
            Blob = new BlobDto();
        }
    }
}
