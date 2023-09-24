using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using LibraryAPI.Controllers;
using BCrypt.Net;
using MySqlConnector;
using LibraryAPI.Models;

[Route("api/authentication")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AuthenticationController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDTO loginDTO)
    {
        try
        {
            using MySqlConnection dbConnection = new MySqlConnection("Server=localhost;Database=library;Uid=root;Pwd=jujuamapi0504");
            dbConnection.Open();

            // Check if the provided username exists in the user_login table.
            var user = dbConnection.QuerySingleOrDefault<UserLogin>("SELECT * FROM user_login WHERE username = @Username",
                new { loginDTO.Username });

            if (user == null)
            {
                return Unauthorized(); // Invalid credentials
            }

            // Verify the provided password against the stored hashed password.
            if (BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.PasswordHash))
            {
                // Passwords match; generate an authentication token here (securely).

                var authToken = Guid.NewGuid().ToString(); // Generate a secure auth token (replace this with a proper method)

                try
                {

                    var existingSession = dbConnection.QueryFirstOrDefault<int>("SELECT COUNT(*) FROM user_sessions WHERE UserId = @UserId",
                new { UserId = user.UserId });

                    var userSession = new UserSession
                    {
                        UserId = user.UserId,
                        AuthToken = authToken,
                        ExpirationTime = DateTime.Now.AddDays(1)
                    };

                    string insertQuery = @"INSERT INTO user_sessions (UserId, AuthToken, ExpirationTime)
                                             VALUES (@UserId, @AuthToken, @ExpirationTime)";

                    if (existingSession > 0)
                    {
                       insertQuery = @"UPDATE user_sessions
                                        SET AuthToken = @AuthToken, 
                                                ExpirationTime = @ExpirationTime
                                                     WHERE UserId = @UserID;";
                    }

                    int sessionId = dbConnection.Execute(insertQuery, userSession);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, ex.Message);
                }


                // Store the authToken in the user_sessions table (create a new record or update an existing one).

                // Return the authToken to the client.
                return Ok(new { AuthToken = authToken });
            }
            else
            {
                return Unauthorized(); // Passwords don't match
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
