using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GameShopWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public UserController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            //provjera za ime i prezime
            if(string.IsNullOrEmpty(user.Firstname) || string.IsNullOrEmpty(user.Lastname))
            {
                return BadRequest("Ime i prezime moraju biti zadani");
            }
            //provjera za lozinku
            if(user.Password.Length < 8)
            {
                return BadRequest("Lozinka mora imati najmanje 8 znakova");
            }
            if(user.Password != user.Password2)
            {
                return BadRequest("Lozinke ne odgovaraju");
            }
            //Provjera emaila
            string sql = "SELECT COUNT(*) broj FROM [User] WHERE email = @email";
            using (var connection = new SqlConnection(configuration.GetConnectionString("connString")))
            {
                int broj = connection.ExecuteScalar<int>(sql, new { email = user.Email });
                if (broj > 0)
                {
                    return BadRequest("Već postoji korisnik s tim emailom");
                }
                sql = @"INSERT INTO [User](firstname, lastname, address, email, password, City)
                VALUES(@firstname, @lastname, @address, @email, @password, @City)";
                connection.Execute(sql, user);
                return Ok();
            }
        }
    }
}
