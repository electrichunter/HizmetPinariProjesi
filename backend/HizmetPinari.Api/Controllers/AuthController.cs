using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HizmetPinari.Api.Data;
using HizmetPinari.Api.Dtos;
using HizmetPinari.Api.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
namespace HizmetPinari.Api.Controllers;
[ApiController, Route("api/[controller]")]
public class AuthController : ControllerBase {
    private readonly ApplicationDbContext _context;
    public AuthController(ApplicationDbContext context) { _context = context; }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request) {
        var user = await _context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash)) {
            return Unauthorized(new { Message = "Geçersiz e-posta veya şifre." });
        }
        var claims = new List<Claim> {
            new(ClaimTypes.NameIdentifier, user.UserID.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
        };
        foreach (var userRole in user.UserRoles) { claims.Add(new Claim(ClaimTypes.Role, userRole.Role.RoleName)); }
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        var userRoles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList();
        return Ok(new LoginResponseDto { Email = user.Email, FullName = $"{user.FirstName} {user.LastName}", Roles = userRoles });
    }
    [HttpPost("logout"), Authorize]
    public async Task<IActionResult> Logout() {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { Message = "Çıkış başarılı." });
    }
    [HttpGet("me"), Authorize]
    public IActionResult GetCurrentUserProfile() {
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        var userRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
        return Ok(new { Email = userEmail, Roles = userRoles, Message = "Bu korumalı bir alandır." });
    }
}