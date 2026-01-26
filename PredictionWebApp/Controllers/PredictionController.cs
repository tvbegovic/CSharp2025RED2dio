using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace PredictionWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PredictionController : ControllerBase
    {
        private IConfiguration configuration;
        public PredictionController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet("")]
        public List<Prediction> GetPredictions(string? username = null)
        {
            string sql = @"SELECT TOP 100 p.*, CONCAT(t1.Name, ' - ', t2.Name, ' ', CONVERT(varchar(30),m.MatchDate,104)) AS Match, pt.Name AS PredictionType
            FROM Prediction p
            LEFT OUTER JOIN Match m ON p.MatchId = m.Id
            LEFT OUTER JOIN Team t1 ON m.HomeTeamId = t1.Id
            LEFT OUTER JOIN Team t2 ON m.AwayTeamId = t2.Id
            LEFT OUTER JOIN PredictionType pt ON p.PredictionTypeId = pt.Id
            WHERE (@userName IS NULL OR p.UserName LIKE CONCAT('%',@userName,'%'))
            ORDER BY p.CreatedAt DESC";
            using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("connString")))
            {
                return connection.Query<Prediction>(sql, new { username
                }).ToList();
            }
        }

        [HttpGet("types")]
        public List<PredictionType> GetTypes()
        {
            using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("connString")))
            {
                string sql = "SELECT * FROM PredictionType";
                return connection.Query<PredictionType>(sql).ToList();
            }
        }

        [HttpPost("")]
        public IActionResult CreateNew(Prediction prediction) 
        { 
            //Validacija
            //primjer tip
            if(prediction.PredictionTypeId == null)
            {
                return BadRequest("Tip prognoze mora biti zadan");
            }
            string sql = @"INSERT INTO [dbo].[Prediction]
                               ([MatchId]
                               ,[UserName]
                               ,[PredictionTypeId]
                               ,[CreatedAt])
                         VALUES
                               (@MatchId 
                               ,@UserName
                               ,@PredictionTypeId
                               ,@CreatedAt)";
            using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("connString")))
            {
                connection.Execute(sql, prediction);
                return Ok();
            }
        }

        [HttpPut("")]
        public IActionResult Update(Prediction prediction)
        {
            //validacija TODO
            string sql = @"UPDATE [dbo].[Prediction]
               SET [MatchId] = @MatchId
                  ,[UserName] = @UserName
                  ,[PredictionTypeId] = @PredictionTypeId
                  ,[CreatedAt] = @CreatedAt
             WHERE Id = @Id";
            using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("connString")))
            {
                connection.Execute(sql, prediction);
                return Ok();
            }
        }
    }
}
