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
    [Authorize] // This attribute requires authentication for this endpoint
    public IActionResult GetUserData([FromQuery(Name = "auth_token")] string authToken)
    {
        try
        {
            // Retrieve the authenticated user's ID from the token (you should have this logic in place)
            var userId = GetUserFromToken(authToken); // Pass the auth_token as a parameter

            using MySqlConnection dbConnection = new MySqlConnection("Server=localhost;Database=library;Uid=root;Pwd=jujuamapi0504");
            dbConnection.Open();

            // Note: You should use parameterized queries to prevent SQL injection
            var userData = dbConnection.QuerySingleOrDefault<UserDataModel>("SELECT * FROM user_data WHERE user_id = @UserId",
                new { UserId = userId });

            if (userData == null)
            {
                return NotFound("User data not found.");
            }

            return Ok(userData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // Implement a method to extract the user ID from the authentication token
    private int GetUserFromToken(string authToken)
    {
        // Replace with your actual logic to extract user ID from the token
        // For simplicity, I'm assuming the token contains the user ID as an integer
        if (int.TryParse(authToken, out int userId))
        {
            return userId;
        }
        else
        {
            throw new ArgumentException("Invalid auth_token format");
        }
    }
}
