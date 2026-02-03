using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SensorHubWeb.Controllers
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

        

        [HttpGet("login")]
        public IActionResult Login(string email, string password)
        {
            string sql = "SELECT * FROM [AppUser] WHERE email = @email AND password = @password";
            using (var connection = new SqlConnection(configuration.GetConnectionString("connString")))
            {
                AppUser user = connection.QueryFirstOrDefault<AppUser>(sql, new { email, password });
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
        public AppUser User { get; set; }
        public string AccessToken { get; set; }
    }
}
