using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using MySqlConnector;

[Route("api/userdata")]
[ApiController]
public class UserDataController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public UserDataController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("m")]
    public async Task<IActionResult> GetUserData([FromQuery(Name = "auth_token")] string authToken)
    {
        try
        {
            // Retrieve the authenticated user's ID from the token (you should have this logic in place)
            var userId = GetUserIdFromToken(authToken); // Pass the auth_token as a parameter

            if (userId == -1) return NotFound("UserId not found.");

            using MySqlConnection dbConnection = new MySqlConnection("Server=localhost;Database=library;Uid=root;Pwd=jujuamapi0504");
            dbConnection.Open();

            // Note: You should use parameterized queries to prevent SQL injection
            var userData = dbConnection.QuerySingleOrDefault<UserDataModel>("SELECT * FROM user_data WHERE user_id = @UserId",
                new { UserId = userId });

            if (userData == null)
            {
                return NotFound($"User data for id {userId} not found.");
            }

            userData.UserId = userId;

            return Ok(userData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("profilepic/{profilePic}")]
    public async Task<IActionResult> GetCover(string profilepic)
    {
        string path = @"C:\ProfilePics\";

        if (System.IO.File.Exists(path + profilepic + ".jpg"))
        {
            try
            {
                var stream = new FileStream(path + profilepic + ".jpg", FileMode.Open, FileAccess.Read);
                var fileStreamResult = new FileStreamResult(stream, "application/octet-stream")
                {
                    FileDownloadName = profilepic + ".jpg",
                };

                return fileStreamResult;
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }
        else
        {
            return NotFound("The file does not exist.");
        }

    }

    // Implement a method to extract the user ID from the authentication token
    private int GetUserIdFromToken(string authToken)
    {
        try
        {
            using MySqlConnection dbConnection = new MySqlConnection("Server=localhost;Database=library;Uid=root;Pwd=jujuamapi0504");
            dbConnection.Open();

            int userId = dbConnection.QuerySingleOrDefault<int>("SELECT UserId FROM user_sessions WHERE AuthToken = @AuthToken",
                new { AuthToken = authToken });
            return userId;
        }
        catch (Exception ex)
        {
            return -1;
        }
    }
}
