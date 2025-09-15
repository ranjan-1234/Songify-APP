namespace Singer.Models
{
    public class SongDto
    {
        public int SongId { get; set; }

        public int UserId { get; set; }  // Added UserId

        public string FileName { get; set; } = null!;  // Default string, max length applied in DbContext
        public string FilePath { get; set; } = null!;  // Default string, max length applied in DbContext

        public DateTime UploadedAt { get; set; }
    }
}
