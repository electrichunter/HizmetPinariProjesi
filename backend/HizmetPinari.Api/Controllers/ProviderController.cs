// Bu dosya, 'Hizmet Veren' rolündeki kullanıcıların kayıt işlemlerini yönetir.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HizmetPinari.Api.Data;
using HizmetPinari.Api.Dtos;
using HizmetPinari.Api.Models;

namespace HizmetPinari.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProviderController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProviderController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] ProviderRegisterDto r)
    {
        if (await _context.Users.AnyAsync(u => u.Email == r.Email))
        {
            return Conflict(new { Message = "Bu e-posta adresi zaten kullanılıyor." });
        }

        var providerRole = await _context.Roles.FirstOrDefaultAsync(x => x.RoleName == "Hizmet Veren");
        if (providerRole == null)
        {
            return StatusCode(500, new { Message = "'Hizmet Veren' rolü bulunamadı." });
        }

        // --- YENİ VE DAHA TEMİZ YÖNTEM ---

        // 1. Ana kullanıcı nesnesini oluştur.
        var user = new User
        {
            Email = r.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(r.Password),
            FirstName = r.FirstName,
            LastName = r.LastName
        };

        // 2. İlişkili nesneleri oluştur ve ana nesneye bağla.
        user.ProviderProfil = new ProviderProfile(); // Boş profil oluştur ve kullanıcıya bağla.
        user.UserRoles.Add(new UserRole { Role = providerRole }); // Rolü kullanıcıya bağla.

        // 3. Sadece ana nesneyi contexte ekle. EF Core diğerlerini otomatik olarak bulacaktır.
        await _context.Users.AddAsync(user);

        // 4. Tüm değişiklikleri tek bir atomik işlemle kaydet.
        await _context.SaveChangesAsync();
        
        return Ok(new { Message = "Hizmet veren kaydı başarıyla oluşturuldu." });
    }
}
