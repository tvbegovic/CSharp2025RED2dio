using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace SensorHubWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IssueController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public IssueController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost("")]
        public IActionResult ReportIssue(Issue issue) 
        {
            if(string.IsNullOrEmpty(issue.Description))
            {
                return BadRequest("Description mora biti zadan");
            }
            string sql = "SELECT * FROM Sensor WHERE id = @id";
            using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("connString")))
            {
                Sensor sensor = connection.QueryFirstOrDefault<Sensor>(sql, new { id =  issue.SensorId });
                if ((sensor == null))
                {
                    return BadRequest("Sensor s tim id-em ne postoji u bazi");
                }
                if(issue.ReportedAt == null)
                {
                    return BadRequest("ReportedAt mora biti zadan");
                }
                if(issue.Severity < 1 || issue.Severity > 5)
                {
                    return BadRequest("Severity mora biti u rasponu 1-5");
                }
                sql = @"INSERT INTO Issue(
                    SensorId,ReportedAt,ReportedBy,Severity,Description
                    ) OUTPUT inserted.id VALUES(
                    @SensorId,@ReportedAt,@ReportedBy,@Severity,@Description
                    )";
                connection.Execute(sql, issue);
                return Ok();
            }
        }
    }
}
