using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GameShopWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public GameController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet("genres")]
        public List<Genre> GetGenres()
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("connString")))
            {
                string sql = "SELECT * FROM Genre";
                return connection.Query<Genre>(sql).ToList();
            }
        }

        [HttpGet("companies")]
        public List<Company> GetCompanies()
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("connString")))
            {
                string sql = "SELECT * FROM Company";
                return connection.Query<Company>(sql).ToList();
            }
        }

        [HttpGet("bygenre/{id}")]
        public List<Game> GetByGenre(int id)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("connString")))
            {
                string sql = "SELECT * FROM Game WHERE idGenre = @id";
                return connection.Query<Game>(sql, new { id }).ToList();
            }
        }

        [HttpGet("bycompany/{id}")]
        public List<Game> GetByCompany(int id)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("connString")))
            {
                string sql =
                    @"SELECT * FROM Game 
                      WHERE idPublisher = @id OR idDeveloper = @id";
                return connection.Query<Game>(sql, new { id }).ToList();
            }
        }

        [HttpGet("search/{text}")]
        public List<Game> Search(string text)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("connString")))
            {
                string sql = @"SELECT * 
                FROM Game 
                LEFT OUTER JOIN Genre ON Game.idGenre = Genre.id
                LEFT OUTER JOIN Company Developer ON Game.idDeveloper = Developer.id
                LEFT OUTER JOIN Company Publisher ON Game.idPublisher = Publisher.id
                WHERE title LIKE @text OR Genre.name LIKE @text OR Developer.name LIKE @text
                OR Publisher.name LIKE @text";
                string textSaPostocima = $"%{text}%";
                return connection.Query<Game>(sql, new { text = textSaPostocima }).ToList();
            }
        }

        [HttpGet("filterByPrices")]
        public List<Game> FilterByPrices(double? from, double? to)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("connString")))
            {
                string sql = @"SELECT * FROM Game
                WHERE (price >= @from OR @from IS NULL) AND
	                (price <= @to OR @to IS NULL)";
                return connection.Query<Game>(sql, new { from, to }).ToList();
            }
        }
    }
}
