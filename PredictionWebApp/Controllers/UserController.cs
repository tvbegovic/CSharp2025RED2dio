using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PredictionWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private JWTTokenConfig _jwtTokenConfig;
        private IConfiguration configuration;

        public UserController(IConfiguration configuration, JWTTokenConfig jWTTokenConfig)
        {
            this.configuration = configuration;
            this._jwtTokenConfig = jWTTokenConfig;
        }

        [HttpGet("login")]
        public IActionResult Login(string email, string password)
        {
            string sql = "SELECT * FROM [User] WHERE email = @email AND password = @password";
            using(SqlConnection connection = new SqlConnection(configuration.GetConnectionString("connString")))
            {
                User user = connection.QueryFirstOrDefault<User>(sql, new { email, password });
                if (user == null)
                {
                    return BadRequest("Ne postoji korisnik s tim emailom i lozinkom");
                }
                LoginResult loginResult = new LoginResult();
                user.Password = null;
                loginResult.User = user;
                loginResult.AccessToken = GenerateToken(email, "user");
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
