using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Singer.Models;

namespace Singer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopSongsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetTopSongs()
        {
            var csvFile = Path.Combine(
                Environment.CurrentDirectory,
                "Indian_Songs",
                "indian_songs_spotify.csv"
            );

            if (!System.IO.File.Exists(csvFile))
                return NotFound("CSV file not found");

            using var reader = new StreamReader(csvFile);
            using var csv = new CsvReader(
                reader,
                new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    PrepareHeaderForMatch = args => args.Header.Trim()
                }
            );

            var records = csv.GetRecords<SongRecord>()
                             .OrderByDescending(x => x.Popularity)
                             .Take(50)
                             .ToList();

            return Ok(records);
        }
    }
}
