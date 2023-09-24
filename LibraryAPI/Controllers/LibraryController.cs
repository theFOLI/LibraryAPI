using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySqlConnector;
using Newtonsoft.Json;
using System.IO;
using System.Net.Sockets;
using System.Xml;
using static System.Net.Mime.MediaTypeNames;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LibraryAPI
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibraryController : ControllerBase
    {



        // GET: api/<ValuesController>
        /*[HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }*/

        // GET api/<ValuesController>/5
        [HttpGet]
        public string Get()
        {
            //string json = "{\r\n  \"books\": [\r\n    {\r\n      \"title\": \"Universidade Marketing Digital\",\r\n      \"author\": \"Alessandro Gerardi\",\r\n      \"coverImagePath\": \"market.jpg\"\r\n    },\r\n    {\r\n      \"title\": \"Espada de vidro\",\r\n      \"author\": \"Victoria Aveyard\",\r\n      \"coverImagePath\": \"espad.jpg\"\r\n    },\r\n\t{\r\n      \"title\": \"O Alquimista\",\r\n      \"author\": \"Paulo Coelho\",\r\n      \"coverImagePath\": \"alquimia.jpg\"\r\n    },\r\n\t{\r\n      \"title\": \"Gatos Guerreiros\",\r\n      \"author\": \"Erin Hunter\",\r\n      \"coverImagePath\": \"gato.jpg\"\r\n    },\r\n\t{\r\n      \"title\": \"Ventos de Mudança\",\r\n      \"author\": \"Beverly Jenkins\",\r\n      \"coverImagePath\": \"vento.jpg\"\r\n    },\r\n\t{\r\n      \"title\": \"Claudinei Prieto\",\r\n      \"author\": \"O novo tarô de Marselha\",\r\n      \"coverImagePath\": \"marsela.jpg\"\r\n    },\r\n\t{\r\n      \"title\": \"As Cronicas de Narnia\",\r\n      \"author\": \"C.S Lewis\",\r\n      \"coverImagePath\": \"narnia.jpg\"\r\n    },\r\n\t{\r\n      \"title\": \"favoRITA\",\r\n      \"author\": \"unknown\",\r\n      \"coverImagePath\": \"rita.jpg\"\r\n    }\r\n  ]\r\n}";

            string connectionString = "Server=localhost;Database=library;Uid=root;Pwd=jujuamapi0504"; // Replace with your MySQL connection string

            List<Book> books = new List<Book>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT * FROM books";
                MySqlCommand command = new MySqlCommand(query, connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Book book = new Book
                        {
                            Id = reader.GetInt32("id"),
                            Title = reader.GetString("title"),
                            Author = reader.GetString("author"),
                            CoverImagePath = reader.GetString("coverImagePath")
                        };

                        books.Add(book);
                    }
                }
            }

            // Convert the list of books to JSON
            string json = JsonConvert.SerializeObject(books, Newtonsoft.Json.Formatting.Indented);

            return json;
        }

        [HttpPost("{id}/update-title")]
        public async Task<IActionResult> UpdateBookTitle(int id, [FromBody] string newTitle)
        {
            try
            {
                using MySqlConnection connection = new MySqlConnection("Server=localhost;Database=library;Uid=root;Pwd=jujuamapi0504");
                await connection.OpenAsync();

                string query = "UPDATE books SET title = @newTitle WHERE id = @id";
                using MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@newTitle", newTitle);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                {
                    return Ok("Title updated successfully.");
                }
                else
                {
                    return NotFound("Book not found.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost("{id}/update-author")]
        public async Task<IActionResult> UpdateBookAuthor(int id, [FromBody] string newAuthor)
        {
            try
            {
                using MySqlConnection connection = new MySqlConnection("Server=localhost;Database=library;Uid=root;Pwd=jujuamapi0504");
                await connection.OpenAsync();

                string query = "UPDATE books SET author = @newAuthor WHERE id = @id";
                using MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@newAuthor", newAuthor);

                int rowsAffected = await command.ExecuteNonQueryAsync();
                if (rowsAffected > 0)
                {
                    return Ok("Author updated successfully.");
                }
                else
                {
                    return NotFound("Book not found.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }       


        // POST api/<ValuesController>
                [HttpPost]
        public void Post([FromBody] string value)
        {
            
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        [HttpGet("cover/{cover}")]
        public async Task<IActionResult> GetCover(string cover)
        {
            string path = @"C:\BookCovers\";

            if (System.IO.File.Exists(path + cover + ".jpg"))
            {
                try
                {
                    var stream = new FileStream(path + cover + ".jpg", FileMode.Open, FileAccess.Read);
                    var fileStreamResult = new FileStreamResult(stream, "application/octet-stream")
                    {
                        FileDownloadName = cover + ".jpg",
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

    }
}
