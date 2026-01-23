using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace PredictionWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private IConfiguration configuration;
        public TeamController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet("")]
        public List<Team> GetTeams()
        {
            string sql = "SELECT * FROM Team";
            using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("connString")))
            {
                return connection.Query<Team>(sql).ToList(); 
            }
        }
    }
}
