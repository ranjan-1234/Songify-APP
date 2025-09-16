using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Globalization;
using Singer.Models;

namespace Singer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TopSongsController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public TopSongsController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet]
        public IActionResult GetTopSongs()
        {
            // Path inside wwwroot/data
            var csvFile = Path.Combine(
                _env.WebRootPath,   // wwwroot folder
                "data",
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
