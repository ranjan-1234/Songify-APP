using CsvHelper.Configuration.Attributes;

namespace Singer.Models
{
    public class SongRecord
    {
        [Name("Song Name")]
        public string Name { get; set; }

        [Name("Artists")]
        public string Artist { get; set; }

        [Name("Released Dates")]
        public string ReleasedDate { get; set; }

        [Name("Popularity")]
        public int Popularity { get; set; }

        [Name("Duration")]
        public string Duration { get; set; }

        [Name("Album Type")]
        public string AlbumType { get; set; }

        [Name("Cover Image")]
        public string CoverImage { get; set; }
    }
}
