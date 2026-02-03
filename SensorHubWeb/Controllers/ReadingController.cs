using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace SensorHubWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReadingController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public ReadingController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet("search")]
        public List<Reading> Search(DateTime? from = null, DateTime? to = null,
            double? minValue = null, double? maxValue = null)
        {
            string sql = @"SELECT * FROM Reading
                WHERE (MeasuredAt >= @from OR @from IS NULL) AND
                (MeasuredAt <= @to OR @to IS NULL) AND
                (value >= @minValue OR @minValue IS NULL) AND
                (value <= @maxValue OR @maxValue IS NULL)";
            using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("connString")))
            {
                return connection.Query<Reading>(sql, new { from, to, minValue, maxValue }).ToList();
            }
        }

        [HttpGet("{sensorId}")]
        public List<Reading> GetReadingsBySensorId(int sensorId)
        {
            string sql = "SELECT * FROM Reading WHERE SensorId = @sensorId";
            using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("connString")))
            {
                return connection.Query<Reading>(sql, new { sensorId }).ToList();
            }
        }
    }
}
