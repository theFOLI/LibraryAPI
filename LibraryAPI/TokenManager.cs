using Dapper;
using LibraryAPI.Models;
using MySqlConnector;

namespace LibraryAPI
{
    public class TokenManager
    {
        public static bool checkTokenValidity(string authToken)
        {
            try
            {
                using MySqlConnection dbConnection = new MySqlConnection("Server=localhost;Database=library;Uid=root;Pwd=jujuamapi0504");
                dbConnection.Open();

                // Check if the provided username exists in the user_login table.
                var session = dbConnection.QuerySingleOrDefault<UserSession>("SELECT * FROM user_sessions WHERE username = @AuthToken",
                    new {AuthToken = authToken});

                if (DateTime.Now > session.ExpirationTime)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
