using dokumansistem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dokumansistem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _context;

        public UsersController(UserContext context)
        {
            _context = context;
        }

        // POST: api/Users/Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginModel.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginModel.Password, user.Password))
            {
                return Unauthorized("Invalid email or password");
            }

            // Kullanıcı doğrulandıktan sonra yapılacak işlemler
            return Ok("Login successful");
        }

        private bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            // Hash'lenmiş şifreyi karşılaştırıyoruz
            return enteredPassword == storedPassword;  // Burada daha gelişmiş bir hash karşılaştırması yapılmalı
        }
    }
}