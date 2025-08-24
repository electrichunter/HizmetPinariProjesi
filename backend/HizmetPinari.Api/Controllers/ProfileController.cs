// Bu dosya, Hizmet Veren profillerini görüntüleme ve güncelleme işlemlerini yönetir.
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HizmetPinari.Api.Data;
using HizmetPinari.Api.Dtos;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace HizmetPinari.Api.Controllers;

[ApiController]
[Route("api/profiles")]
public class ProfileController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProfileController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Herkesin bir hizmet veren profilini görüntüleyebilmesi için herkese açık endpoint
    [HttpGet("{userId}")] // GET /api/profiles/123
    public async Task<IActionResult> GetProfile(long userId)
    {
        var profile = await _context.ProviderProfiles
            .Include(p => p.User) // User tablosundaki bilgileri de çek
            .FirstOrDefaultAsync(p => p.UserID == userId);

        if (profile == null)
        {
            return NotFound(new { Message = "Profil bulunamadı." });
        }

        var profileDto = new ProviderProfileDto
        {
            UserId = profile.UserID,
            FullName = $"{profile.User.FirstName} {profile.User.LastName}",
            Email = profile.User.Email,
            Bio = profile.Bio,
            Expertise = profile.Expertise,
            PortfolioUrl = profile.PortfolioUrl,
            Location = profile.Location
        };

        return Ok(profileDto);
    }

    // Sadece giriş yapmış ve 'Hizmet Veren' rolündeki kullanıcının KENDİ profilini güncellemesi için
    [HttpPut("me")] // PUT /api/profiles/me
    [Authorize(Roles = "Hizmet Veren")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProviderProfileDto dto)
    {
        // Giriş yapmış kullanıcının ID'sini cookie'den alıyoruz
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var profileToUpdate = await _context.ProviderProfiles.FindAsync(userId);
        if (profileToUpdate == null)
        {
            return NotFound(new { Message = "Güncellenecek profil bulunamadı." });
        }

        profileToUpdate.Bio = dto.Bio;
        profileToUpdate.Expertise = dto.Expertise;
        profileToUpdate.PortfolioUrl = dto.PortfolioUrl;
        profileToUpdate.Location = dto.Location;

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Profil başarıyla güncellendi." });
    }
}
