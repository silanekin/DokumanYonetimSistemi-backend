using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using DokumanSistem.Core.Models;
using DokumanSistem.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DokumanSistem.API
{


    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DocumentDbContext _context;

        public UserController(DocumentDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        [Route("api/users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        private string ComputeSha512Hash(string rawData)
        {
            using (SHA512 sha512Hash = SHA512.Create())
            {
                byte[] bytes = sha512Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("Kullanıcı verisi eksik.");
            }

            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(user, serviceProvider: null, items: null);
            if (!Validator.TryValidateObject(user, context, validationResults, validateAllProperties: true))
            {
                return BadRequest(validationResults);
            }

            var existingUser = _context.Users.SingleOrDefault(u => u.Email == user.Email);
            if (existingUser != null)
            {
                return Conflict("Bu e-posta adresi zaten kayıtlı.");
            }

            user.Password = ComputeSha512Hash(user.Password);
            user.CreatedDate = DateTime.UtcNow;
            user.CreatedBy = "current";
            user.UpdatedDate = DateTime.UtcNow;
            user.UpdatedBy = "Current";

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new { message = "Kullanıcı başarıyla kaydedildi." });

        }



        [HttpPost("login")]
        public IActionResult Login(LoginModel loginModel)
        {
            
            string hashedPassword = ComputeSha512Hash(loginModel.Password);

       
            var user = _context.Users.FirstOrDefault(u => u.Password == hashedPassword);

            if (user == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new { message = "Kullanıcı bilgileri doğrulanamadı." });
            }

            var token = GenerateJwtToken(user.Password); 
            return Ok(new { token });
        }

       

      private string GenerateJwtToken(string password)
{
  
    var user = _context.Users.FirstOrDefault(u => u.Password == password);

    if (user == null)
    {
        throw new Exception("Kullanıcı bulunamadı.");
    }

    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.ASCII.GetBytes("bu-cok-gizli-ve-uzun-bir-key-256bit");


    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Password),
        new Claim(ClaimTypes.GivenName, user.FullName),
        new Claim(ClaimTypes.Email, user.Email),  
        new Claim("title", user.Title)
    };

    if (user.Title == "Admin")
    {
        claims.Add(new Claim(ClaimTypes.Role, "Admin"));
    }

    
    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddHours(1), 
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };


    var token = tokenHandler.CreateToken(tokenDescriptor);
    return tokenHandler.WriteToken(token);
}

        [Authorize]
        [HttpGet("admin-data")]
        public IActionResult GetAdminData()
        {
            var userTitle = User.Claims.FirstOrDefault(c => c.Type == "title")?.Value;

            if (userTitle != "Admin")
            {
                return Forbid("Bu işlemi sadece admin yetkisine sahip kullanıcılar yapabilir.");
            }

            var users = _context.Users.ToList();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("user-data")]
        public IActionResult GetUserData()
        {
            var userTitle = User.Claims.FirstOrDefault(c => c.Type == "title")?.Value;

            if (userTitle == "Admin")
            {
                return Forbid("Admin kullanıcıları bu veriye erişemez.");
            }

            return Ok(new { message = "Bu veri admin dışındaki tüm kullanıcılar için!" });
        }
    }
}


