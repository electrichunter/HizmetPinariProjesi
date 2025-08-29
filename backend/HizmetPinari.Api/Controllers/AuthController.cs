
using HizmetPinari.Api.Data;
using HizmetPinari.Api.Dtos;
using HizmetPinari.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
// JWT için gerekli kütüphaneler eklenecek (using System.IdentityModel.Tokens.Jwt; vs.)

namespace HizmetPinari.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("Bu email adresi zaten kullanılıyor.");
            }

            // Şifre hash'leme işlemi burada yapılmalı (örn: BCrypt.Net.BCrypt.HashPassword)
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
            };

            // Varsayılan olarak her kullanıcı hem 'Müşteri' hem 'Hizmet Veren' olsun
            var customerRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Müşteri");
            var providerRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Hizmet Veren");

            if (customerRole != null)
                user.UserRoles.Add(new UserRole { Role = customerRole });
            if (providerRole != null)
                user.UserRoles.Add(new UserRole { Role = providerRole });


            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kullanıcı başarıyla oluşturuldu." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto request)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized("Geçersiz email veya şifre.");
            }

            // JWT (JSON Web Token) oluşturma işlemi burada yapılacak
            // Token oluşturulduktan sonra AuthResponseDto ile geri döndürülecek
            
            // Örnek JWT oluşturma mantığı (gerçek kod daha karmaşık olacaktır)
            var token = "YAPILACAK: " + user.Email; // Burası gerçek token ile değiştirilecek
            
            var userDto = new UserDto {
                UserId = user.UserID,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList()
            };

            return Ok(new AuthResponseDto { Token = token, User = userDto });
        }
    }
}
