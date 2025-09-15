using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Singer.Helpers;
using System.Data;

[ApiController]
[Route("api/[controller]")]
public class SongApiController : ControllerBase
{
    private readonly DatabaseHelper _db;
    private readonly IWebHostEnvironment _env;

    public SongApiController(DatabaseHelper db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    [HttpPost("Upload")] // endpoint: api/SongApi/Upload
    [ApiExplorerSettings(IgnoreApi = true)] // Hide from Swagger
    public IActionResult UploadSong([FromForm] IFormFile file, [FromForm] int userId)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        if (!file.FileName.EndsWith(".wav"))
            return BadRequest("Only .wav files are allowed.");

        string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        string filePath = Path.Combine(uploadsFolder, file.FileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            file.CopyTo(stream);
        }

        string query = "INSERT INTO Songs (UserId, FileName, FilePath, UploadedAt) VALUES (@UserId, @FileName, @FilePath, @UploadedAt)";
        _db.ExecuteNonQuery(query, new SqlParameter[]
        {
            new SqlParameter("@UserId", userId),
            new SqlParameter("@FileName", file.FileName),
            new SqlParameter("@FilePath", filePath),
            new SqlParameter("@UploadedAt", DateTime.Now)
        });

        return Ok("Song uploaded successfully!");
    }

    [HttpGet("Playlist/{userId}")]
    public IActionResult GetUserPlaylist(int userId)
    {
        string query = "SELECT SongId, FileName, FilePath, UploadedAt FROM Songs WHERE UserId=@UserId";
        var dt = _db.ExecuteSelectQuery(query, new SqlParameter[] { new SqlParameter("@UserId", userId) });

        var songs = dt.AsEnumerable().Select(row => new {
            SongId = row.Field<int>("SongId"),
            FileName = row.Field<string>("FileName"),
            FilePath = row.Field<string>("FilePath"),
            UploadedAt = row.Field<DateTime>("UploadedAt")
        }).ToList();

        return Ok(songs); // This will return proper JSON
    }


    [HttpDelete("Delete/{songId}")]
    public IActionResult DeleteSong(int songId)
    {
        string selectQuery = "SELECT FilePath FROM Songs WHERE SongId=@SongId";
        var dt = _db.ExecuteSelectQuery(selectQuery, new SqlParameter[] { new SqlParameter("@SongId", songId) });

        if (dt.Rows.Count == 0)
            return NotFound("Song not found.");

        string filePath = dt.Rows[0]["FilePath"].ToString();
        if (System.IO.File.Exists(filePath))
            System.IO.File.Delete(filePath);

        string deleteQuery = "DELETE FROM Songs WHERE SongId=@SongId";
        _db.ExecuteNonQuery(deleteQuery, new SqlParameter[] { new SqlParameter("@SongId", songId) });

        return Ok("Song deleted successfully!");
    }
}
