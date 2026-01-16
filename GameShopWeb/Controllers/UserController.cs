using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GameShopWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private JWTTokenConfig _jwtTokenConfig;

        public UserController(IConfiguration configuration, JWTTokenConfig jwtTokenConfig)
        {
            this.configuration = configuration;
            _jwtTokenConfig = jwtTokenConfig;
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
            //Privremeno isključujemo zbog frontenda
            /*if(user.Password != user.Password2)
            {
                return BadRequest("Lozinke ne odgovaraju");
            }*/
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

        [HttpGet("login")]
        public IActionResult Login(string email, string password)
        {
            string sql = "SELECT * FROM [User] WHERE email = @email AND password = @password";
            using (var connection = new SqlConnection(configuration.GetConnectionString("connString")))
            {
                User user = connection.QueryFirstOrDefault<User>(sql, new { email, password });
                if (user == null) 
                {
                    return BadRequest("Ne postoji korisnik s tim emailom ili lozinkom");
                }
                LoginResult loginResult = new LoginResult();
                user.Password = null;
                loginResult.User = user;
                loginResult.AccessToken = GenerateToken(user.Email, "user");
                return Ok(loginResult);
            }

        }

        private string GenerateToken(string email, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(_jwtTokenConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.Role, role)
        }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtTokenConfig.AccessTokenExpiration),
                Issuer = _jwtTokenConfig.Issuer,
                Audience = _jwtTokenConfig.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public class LoginResult
    {
        public User User { get; set; }
        public string AccessToken { get; set; }
    }
}
