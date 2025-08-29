
using HizmetPinari.Api.Data;
using HizmetPinari.Api.Dtos;
using HizmetPinari.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HizmetPinari.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize(Roles = "Admin")] // Bu endpoint'lere sadece Admin rolündekiler erişebilir
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- Destek Personeli Yönetimi ---

        [HttpGet("support")]
        public async Task<IActionResult> GetSupportStaff()
        {
            var supportRoleId = await _context.Roles
                .Where(r => r.RoleName == "Destek")
                .Select(r => r.RoleID)
                .FirstOrDefaultAsync();

            if (supportRoleId == 0) return Ok(new List<UserDto>());

            var supportUsers = await _context.UserRoles
                .Where(ur => ur.RoleID == supportRoleId)
                .Select(ur => new UserDto {
                    UserId = ur.User.UserID,
                    Email = ur.User.Email,
                    FirstName = ur.User.FirstName,
                    LastName = ur.User.LastName
                })
                .ToListAsync();

            return Ok(supportUsers);
        }
        
        [HttpPost("support")]
        public async Task<IActionResult> CreateSupportUser(SupportUserCreateDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("Bu email adresi zaten kullanılıyor.");
            }
            
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var supportRole = await _context.Roles.SingleOrDefaultAsync(r => r.RoleName == "Destek");
            if (supportRole == null) return StatusCode(500, "Destek rolü bulunamadı.");

            var user = new User
            {
                Email = request.Email,
                PasswordHash = passwordHash,
                FirstName = request.FirstName,
                LastName = request.LastName,
            };
            user.UserRoles.Add(new UserRole { Role = supportRole });

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { message = "Destek personeli başarıyla oluşturuldu."});
        }

        [HttpDelete("support/{id}")]
        public async Task<IActionResult> DeleteSupportUser(long id)
        {
             var user = await _context.Users.FindAsync(id);
             if (user == null)
             {
                 return NotFound("Kullanıcı bulunamadı.");
             }
            
             _context.Users.Remove(user);
             await _context.SaveChangesAsync();

             return NoContent(); // Başarılı silme
        }
    }
}
