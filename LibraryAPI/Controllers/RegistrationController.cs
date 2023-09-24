using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using MySqlConnector;

[Route("api/registration")]
[ApiController]
public class RegistrationController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public RegistrationController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDTO registrationDTO)
    {
        try
        {
            using MySqlConnection dbConnection = new MySqlConnection("Server=localhost;Database=library;Uid=root;Pwd=jujuamapi0504");
            dbConnection.Open();

            // Check if the username is already taken
            var existingUser = dbConnection.QueryFirstOrDefault<int>("SELECT COUNT(*) FROM user_login WHERE username = @Username",
                new { registrationDTO.Username });

            if (existingUser > 0)
            {
                return BadRequest("Username is already taken.");
            }

            // Hash the user's password (you can use BCrypt or another secure hashing method)

            string salt = BCrypt.Net.BCrypt.GenerateSalt(12);

            string hashedPassword = HashPassword(registrationDTO.Password, salt);

            // Insert user_login data
            await dbConnection.ExecuteAsync("INSERT INTO user_login (Username, PasswordHash) VALUES (@Username, @PasswordHash)",
                new { Username = registrationDTO.Username, PasswordHash = hashedPassword});

            // Retrieve the user_id of the newly inserted user_login record
            var userId = dbConnection.QuerySingle<int>("SELECT LAST_INSERT_ID()");

            // Insert user_data for the registered user
            dbConnection.Execute(@"INSERT INTO user_data (user_id, name, login, birth_date, sex, email, phone_number, cpf, profile_pic_filename)
                                   VALUES (@UserId, @Name, @Login, @BirthDate, @Sex, @Email, @PhoneNumber, @CPF, @ProfilePicFilename)",
                new
                {
                    UserId = userId,
                    Name = registrationDTO.Name,
                    Login = registrationDTO.Login,
                    BirthDate = registrationDTO.BirthDate,
                    Sex = registrationDTO.Sex,
                    Email = registrationDTO.Email,
                    PhoneNumber = registrationDTO.PhoneNumber,
                    CPF = registrationDTO.CPF,
                    ProfilePicFilename = registrationDTO.ProfilePicFilename
                });

            return Ok("User registered successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    // Hash the password using a secure method (e.g., BCrypt)
    private string HashPassword(string password, string salt)
    {
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

        return hashedPassword;
    }
}
