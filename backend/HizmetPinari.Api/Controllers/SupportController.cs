using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HizmetPinari.Api.Data;
using HizmetPinari.Api.Dtos;
using HizmetPinari.Api.Models;
namespace HizmetPinari.Api.Controllers;
[ApiController, Route("api/[controller]")]
public class SupportController : ControllerBase {
    private readonly IConfiguration _config;
    private readonly ApplicationDbContext _context;
    public SupportController(IConfiguration config, ApplicationDbContext context) { _config = config; _context = context; }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] SupportRegisterDto r) {
        if (r.SupportKey != _config["RegistrationKeys:Support"]) return Unauthorized(new { Message = "Geçersiz destek anahtarı." });
        if (await _context.Users.AnyAsync(u => u.Email == r.Email)) return Conflict(new { Message = "Bu e-posta zaten kullanılıyor." });
        var user = new User { Email = r.Email, PasswordHash = BCrypt.Net.BCrypt.HashPassword(r.Password), FirstName = r.FirstName, LastName = r.LastName };
        var role = await _context.Roles.FirstOrDefaultAsync(x => x.RoleName == "Destek");
        if (role == null) return StatusCode(500, new { Message = "Destek rolü bulunamadı." });
        await _context.Users.AddAsync(user); await _context.SaveChangesAsync();
        await _context.UserRoles.AddAsync(new UserRole { UserID = user.UserID, RoleID = role.RoleID }); await _context.SaveChangesAsync();
        return Ok(new { Message = "Destek kullanıcısı başarıyla oluşturuldu." });
    }
}