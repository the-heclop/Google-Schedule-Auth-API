using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PersonalAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PersonalAPI.Data;
using PersonalAPI.DTOs;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PersonalAPI.Controllers
{    
    [ApiController]
    [Route("account")]
    public class AccountController : ControllerBase
    {

        [HttpGet]
        [Authorize]
        public IEnumerable<string> Get()
        {
            return new string[] 
            {
                "value1",
                "value2" ,
                "teest",
                "test2",
                "burger",
                "fries",
                "coke",
                "sprite",
                "pepsi",
            };
        }

        private readonly DataContext _context;

        public AccountController(DataContext context)
        {
            _context = context;
        }


        [Route("login")]
        [HttpPost]
        public async Task <ActionResult<UserDTO>> Login(LoginDto loginRequest)
        {
            var user = await _context.TokenUsers.SingleOrDefaultAsync(u => u.username == loginRequest.Username);

            if (user == null) return BadRequest("Incorrect login");
            

            bool hash = BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.passwordHash);

            if (loginRequest.Username == user.username && hash)
            {

                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("KeyForSignInSecret@1234"));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

                var tokeOptions = new JwtSecurityToken(
                    issuer: "https://localhost:7226",
                    audience: "https://localhost:7226",
                    claims: new List<Claim>(),
                    expires: DateTime.Now.AddHours(24),
                    signingCredentials: signinCredentials                    
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return new UserDTO { Username = user.username, Token = tokenString };
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(LoginModel newUser)
        {
            if (await UserExist(newUser.username)) return BadRequest("Username already exists");

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("KeyForSignInSecret@1234"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var tokeOptions = new JwtSecurityToken(
                issuer: "https://localhost:7226",
                audience: "https://localhost:7226",
                claims: new List<Claim>(),
                expires: DateTime.Now.AddHours(24),
                signingCredentials: signinCredentials
            );

            await RegisterNewUserHash(newUser);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
            return new UserDTO { Username = newUser.username, Token = tokenString };

        }

        private async Task RegisterNewUserHash(LoginModel user)
        {
            user.passwordHash = BCrypt.Net.BCrypt.HashPassword(user.passwordHash);
            _context.TokenUsers.Add(user);
            await _context.SaveChangesAsync();
        }

        private async Task<bool> UserExist(string username)
        {

            return await _context.TokenUsers.AnyAsync(x => x.username == username.ToLower());
        }

    }
}
