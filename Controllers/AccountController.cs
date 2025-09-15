using Microsoft.AspNetCore.Mvc;
using Singer.Helpers;
using Singer.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace Singer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountApiController : ControllerBase
    {
        private readonly DatabaseHelper _db;

        public AccountApiController(DatabaseHelper db)
        {
            _db = db;
        }

        // POST api/AccountApi/Register
        [HttpPost("Register")]
        public IActionResult Register([FromBody] RegisterRequest input)
        {
            string email = input.Email;
            string password = input.Password;

            var dt = _db.ExecuteSelectQuery(
                "SELECT * FROM Users WHERE Email=@Email",
                new MySqlParameter[] { new MySqlParameter("@Email", email) });

            if (dt.Rows.Count > 0)
                return BadRequest("Email already exists");

            // Generate hash + salt
            PasswordHelper.CreatePasswordHash(password, out byte[] hash, out byte[] salt);

            string insertQuery = "INSERT INTO Users (Email, PasswordHash, PasswordSalt, CreatedAt) " +
                                 "VALUES (@Email, @PasswordHash, @PasswordSalt, @CreatedAt)";

            _db.ExecuteNonQuery(insertQuery, new MySqlParameter[]
            {
                new MySqlParameter("@Email", email),
                new MySqlParameter("@PasswordHash", hash),
                new MySqlParameter("@PasswordSalt", salt),
                new MySqlParameter("@CreatedAt", DateTime.Now)
            });

            return Ok("Registration successful");
        }

        // POST api/AccountApi/Login
        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequest input)
        {
            string email = input.Email;
            string password = input.Password;

            var dt = _db.ExecuteSelectQuery(
                "SELECT * FROM Users WHERE Email=@Email",
                new MySqlParameter[] { new MySqlParameter("@Email", email) });

            if (dt.Rows.Count == 0)
                return BadRequest("Invalid email or password");

            byte[] storedHash = (byte[])dt.Rows[0]["PasswordHash"];
            byte[] storedSalt = (byte[])dt.Rows[0]["PasswordSalt"];

            if (!PasswordHelper.VerifyPassword(password, storedHash, storedSalt))
                return BadRequest("Invalid email or password");

            // ✅ Return message + userId + email
            return Ok(new
            {
                message = "Login successful",
                userId = (int)dt.Rows[0]["UserId"],
                email = email
            });
        }
    }
}
