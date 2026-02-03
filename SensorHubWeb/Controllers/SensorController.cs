using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace SensorHubWeb.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class SensorController : Controller
  {
    private readonly IConfiguration configuration;

    public SensorController(IConfiguration configuration)
    {
      this.configuration = configuration;
    }

    [HttpGet("")]
    public List<Sensor> GetAllSensors()
    {
      string sql = "SELECT * FROM Sensor";
      using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("connString")))
      {
        return connection.Query<Sensor>(sql).ToList();
      }
    }

    [HttpGet("{id}")]
    public Sensor GetSensorById(int id)
    {
      string sql = "SELECT * FROM Sensor WHERE Id = @id";
      using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("connString")))
      {
        return connection.QueryFirstOrDefault<Sensor>(sql, new { id });
      }
    }

    [HttpGet("search/{text}")]
    public List<Sensor> SearchSensors(string text)
    {
      //search in room name, sensor code, type name
      string sql = @"SELECT * FROM Sensor
                  LEFT OUTER JOIN Room ON Sensor.RoomId = Room.Id
                  LEFT OUTER JOIN SensorType ON Sensor.SensorTypeId = SensorType.Id
                  WHERE Sensor.Code LIKE @text OR Room.Name LIKE @text OR SensorType.Name LIKE @text";
      using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("connString")))
      {
        return connection.Query<Sensor>(sql, new { text = $"%{text}%"}).ToList();
      }
    }
  }
}
