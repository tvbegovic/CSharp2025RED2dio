using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace PredictionWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private IConfiguration configuration;
        public MatchController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet("search")]
        public List<Match> Search(int? teamId = null, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            string sql = @"SELECT m.Id, m.MatchDate, m.ProbHomeWin * 100 ProbHomeWin, m.ProbAwayWin * 100 ProbAwayWin,(1 - m.ProbHomeWin - m.ProbAwayWin) * 100 ProbDraw, t1.Name HomeTeam, t2.Name AwayTeam 
            FROM Match m
            LEFT OUTER JOIN Team t1 ON m.HomeTeamId = t1.Id
            LEFT OUTER JOIN Team t2 ON m.AwayTeamId = t2.Id
            WHERE (m.HomeTeamId = @klubId OR m.AwayTeamId = @klubId OR @klubId IS NULL) AND
            (m.MatchDate >= @dateFrom OR @dateFrom IS NULL) AND
            (m.MatchDate <= @dateTo OR @dateTo IS NULL)";
            using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("connString")))
            {
                return connection.Query<Match>(sql, new { klubId = teamId, dateFrom, dateTo } ).ToList();
            }
        }
    }
}
