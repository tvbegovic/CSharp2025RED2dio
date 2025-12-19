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
    }
}
