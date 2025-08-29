// Bu dosya, standart kullanıcıların (Müşteri) kayıt, profil görüntüleme ve güncelleme işlemlerini yönetir.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HizmetPinari.Api.Data;
using HizmetPinari.Api.Dtos; // DTO'larınızın bulunduğu namespace
using HizmetPinari.Api.Models;
using System.Security.Claims;

namespace HizmetPinari.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

   
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto r)
    {
        if (await _context.Users.AnyAsync(u => u.Email == r.Email))
        {
            return Conflict(new { Message = "Bu e-posta adresi zaten kullanılıyor." });
        }

        var customerRole = await _context.Roles.FirstOrDefaultAsync(x => x.RoleName == "Müşteri");
        if (customerRole == null)
        {
            return StatusCode(500, new { Message = "'Müşteri' rolü bulunamadı." });
        }

        var user = new User
        {
            Email = r.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(r.Password),
            FirstName = r.FirstName,
            LastName = r.LastName,
            PhoneNumber = r.PhoneNumber
        };

        user.UserRoles.Add(new UserRole { Role = customerRole });

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        
        return Ok(new { Message = "Kullanıcı kaydı başarıyla oluşturuldu." });
    }

    // --- KİMLİK DOĞRULAMASI GEREKEN ENDPOINT'LER ---

  
    /// Giriş yapmış kullanıcının kendi profil bilgilerini getirir.
     
    [HttpGet("me")]
    [Authorize] // Sadece giriş yapmış kullanıcılar erişebilir.
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var user = await _context.Users
            .Where(u => u.UserID == userId.Value)
            .Select(u => new UserProfileDto // Sadece gerekli bilgileri döndür
            {
            UserId = u.UserID,
            Email = u.Email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            PhoneNumber = u.PhoneNumber,
            ProfilePictureUrl = u.ProfilePictureURL // DİKKAT: Modeldeki property adı 'ProfilePictureURL' olmalı
            })
            .FirstOrDefaultAsync();

        if (user == null)
        {
            return NotFound(new { Message = "Kullanıcı profili bulunamadı." });
        }

        return Ok(user);
    }

    /// <summary>
    /// Giriş yapmış kullanıcının kendi profil bilgilerini günceller.
    /// </summary>
    [HttpPut("me")]
    [Authorize] // Sadece giriş yapmış kullanıcılar erişebilir.
    public async Task<IActionResult> UpdateMyProfile([FromBody] UserDto dto)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var userToUpdate = await _context.Users.FindAsync(userId.Value);
        if (userToUpdate == null)
        {
            return NotFound(new { Message = "Güncellenecek kullanıcı bulunamadı." });
        }

        // Gelen verilerle mevcut kullanıcı bilgilerini güncelle
        userToUpdate.FirstName = dto.FirstName;
        userToUpdate.LastName = dto.LastName;
        userToUpdate.PhoneNumber = dto.PhoneNumber;
        userToUpdate.ProfilePictureURL = dto.ProfilePictureUrl;

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Profil başarıyla güncellendi." });
    }

    // Helper method to get current user's ID from claims
    private long? GetCurrentUserId()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return long.TryParse(userIdString, out long userId) ? userId : (long?)null;
    }
}