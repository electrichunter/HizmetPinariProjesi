# ***************************************************************
# DOSYA: k.py
# AÇIKLAMA: HizmetPınarı API projesini lokal makinede sıfırdan,
#           hatasız bir şekilde kurar ve yapılandırır.
# KULLANIM: python k.py
# ***************************************************************

import os
import shutil
import subprocess
import stat
import errno

# --- PROJE YAPILANDIRMASI ---

API_PROJECT_NAME = "HizmetPinari.Api"
API_DIR = os.path.join(os.getcwd(), API_PROJECT_NAME)

# Oluşturulacak klasörler
DIRECTORIES = [
    "Controllers",
    "Data",
    "Dtos",
    "Models"
]

# .csproj dosyasının içeriği
CSPROJ_CONTENT = """
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.8" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.8" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.8">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="8.0.2" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.4.0" />
  </ItemGroup>

</Project>
"""

# appsettings.json dosyasının içeriği
APPSETTINGS_CONTENT = """
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=hizmet_pinari_db;user=root;password=root_password;Allow User Variables=True"
  },
  "RegistrationKeys": {
    "Admin": "SECRET_ADMIN_KEY_123",
    "Support": "SECRET_SUPPORT_KEY_456"
  }
}
"""

# Oluşturulacak diğer dosyalar ve içerikleri
FILES = {
    "Program.cs": """
using HizmetPinari.Api.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "HizmetPinari.AuthCookie";
        options.ExpireTimeSpan = TimeSpan.FromHours(3);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
""",
    "Models/User.cs": """
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HizmetPinari.Api.Models;
[Table("Users")]
public class User {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] public long UserID { get; set; }
    [Required, EmailAddress, StringLength(255)] public string Email { get; set; } = string.Empty;
    [Required, StringLength(255)] public string PasswordHash { get; set; } = string.Empty;
    [Required, StringLength(100)] public string FirstName { get; set; } = string.Empty;
    [Required, StringLength(100)] public string LastName { get; set; } = string.Empty;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
""",
    "Models/Role.cs": """
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HizmetPinari.Api.Models;
[Table("Roles")]
public class Role {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int RoleID { get; set; }
    [Required, StringLength(50)] public string RoleName { get; set; } = string.Empty;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
""",
    "Models/UserRole.cs": """
using System.ComponentModel.DataAnnotations.Schema;
namespace HizmetPinari.Api.Models;
[Table("UserRoles")]
public class UserRole {
    public long UserID { get; set; }
    public User User { get; set; } = null!;
    public int RoleID { get; set; }
    public Role Role { get; set; } = null!;
}
""",
    "Dtos/AdminRegisterDto.cs": """
namespace HizmetPinari.Api.Dtos;
public class AdminRegisterDto {
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string AdminKey { get; set; } = string.Empty;
}
""",
    "Dtos/SupportRegisterDto.cs": """
namespace HizmetPinari.Api.Dtos;
public class SupportRegisterDto {
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string SupportKey { get; set; } = string.Empty;
}
""",
    "Dtos/LoginRequestDto.cs": """
namespace HizmetPinari.Api.Dtos;
public class LoginRequestDto {
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
""",
    "Dtos/LoginResponseDto.cs": """
namespace HizmetPinari.Api.Dtos;
public class LoginResponseDto {
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new List<string>();
}
""",
    "Data/ApplicationDbContext.cs": """
using HizmetPinari.Api.Models;
using Microsoft.EntityFrameworkCore;
namespace HizmetPinari.Api.Data;
public class ApplicationDbContext : DbContext {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UserRole>().HasKey(ur => new { ur.UserID, ur.RoleID });
        modelBuilder.Entity<UserRole>().HasOne(ur => ur.User).WithMany(u => u.UserRoles).HasForeignKey(ur => ur.UserID);
        modelBuilder.Entity<UserRole>().HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleID);
    }
}
""",
    "Controllers/AdminController.cs": """
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HizmetPinari.Api.Data;
using HizmetPinari.Api.Dtos;
using HizmetPinari.Api.Models;
namespace HizmetPinari.Api.Controllers;
[ApiController, Route("api/[controller]")]
public class AdminController : ControllerBase {
    private readonly IConfiguration _config;
    private readonly ApplicationDbContext _context;
    public AdminController(IConfiguration config, ApplicationDbContext context) { _config = config; _context = context; }
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] AdminRegisterDto r) {
        if (r.AdminKey != _config["RegistrationKeys:Admin"]) return Unauthorized(new { Message = "Geçersiz admin anahtarı." });
        if (await _context.Users.AnyAsync(u => u.Email == r.Email)) return Conflict(new { Message = "Bu e-posta zaten kullanılıyor." });
        var user = new User { Email = r.Email, PasswordHash = BCrypt.Net.BCrypt.HashPassword(r.Password), FirstName = r.FirstName, LastName = r.LastName };
        var role = await _context.Roles.FirstOrDefaultAsync(x => x.RoleName == "Admin");
        if (role == null) return StatusCode(500, new { Message = "Admin rolü bulunamadı." });
        await _context.Users.AddAsync(user); await _context.SaveChangesAsync();
        await _context.UserRoles.AddAsync(new UserRole { UserID = user.UserID, RoleID = role.RoleID }); await _context.SaveChangesAsync();
        return Ok(new { Message = "Admin kullanıcısı başarıyla oluşturuldu." });
    }
}
""",
    "Controllers/SupportController.cs": """
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
""",
    "Controllers/AuthController.cs": """
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
"""
}

GITIGNORE_CONTENT = """
# Build results
[Bb]in/
[Oo]bj/

# User-specific files
*.suo
*.user
*.userosscache
*.sln.docstates

# Visual Studio Code
.vscode/
"""

def handle_remove_readonly(func, path, exc):
    """Hata durumunda salt okunur dosyaları silmeye zorlar."""
    excvalue = exc[1]
    if func in (os.rmdir, os.remove, os.unlink) and excvalue.errno == errno.EACCES:
        os.chmod(path, stat.S_IRWXU| stat.S_IRWXG| stat.S_IRWXO) # 0777
        func(path)
    else:
        raise

def run_command(command, cwd):
    """Terminal komutunu çalıştırır ve hataları kontrol eder."""
    try:
        # Windows'ta shell=True kullanmak genellikle daha güvenilirdir.
        subprocess.run(command, check=True, shell=True, cwd=cwd, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
    except subprocess.CalledProcessError as e:
        print(f"❌ Komut hatası: {e.stderr.decode('utf-8', errors='ignore')}")
        exit(1)

def main():
    """Ana kurulum fonksiyonu."""
    print(f"🚀 '{API_PROJECT_NAME}' projesi lokalde kuruluyor...")

    # --- 1. Adım: Temizleme ---
    if os.path.exists(API_DIR):
        print(f"🧹 Mevcut '{API_PROJECT_NAME}' klasörü siliniyor...")
        shutil.rmtree(API_DIR, onerror=handle_remove_readonly)
        print("✅ Temizleme tamamlandı.")

    # --- 2. Adım: Proje Oluşturma ---
    print(f"🏗️  Yeni .NET Web API projesi '{API_PROJECT_NAME}' oluşturuluyor...")
    run_command(f"dotnet new webapi -n {API_PROJECT_NAME} --no-https", cwd=os.getcwd())
    print("✅ Proje oluşturuldu.")

    # --- 3. Adım: Gereksiz Dosyaları Silme ---
    print("🗑️  Örnek dosyalar siliniyor...")
    for filename in ["Controllers/WeatherForecastController.cs", "WeatherForecast.cs"]:
        filepath = os.path.join(API_DIR, filename)
        if os.path.exists(filepath):
            os.remove(filepath)
    print("✅ Örnek dosyalar silindi.")

    # --- 4. Adım: Klasör ve Dosyaları Oluşturma ---
    print("✍️  Proje yapısı ve dosyalar oluşturuluyor...")
    for directory in DIRECTORIES:
        os.makedirs(os.path.join(API_DIR, directory), exist_ok=True)

    for path, content in FILES.items():
        full_path = os.path.join(API_DIR, path)
        with open(full_path, 'w', encoding='utf-8') as f:
            f.write(content.strip())

    # --- 5. Adım: .csproj ve appsettings.json dosyalarını güncelleme ---
    with open(os.path.join(API_DIR, f"{API_PROJECT_NAME}.csproj"), 'w', encoding='utf-8') as f:
        f.write(CSPROJ_CONTENT.strip())
        
    with open(os.path.join(API_DIR, "appsettings.json"), 'w', encoding='utf-8') as f:
        f.write(APPSETTINGS_CONTENT.strip())

    print("✅ Tüm dosyalar başarıyla oluşturuldu.")

    # --- Bitiş ---
    print("\n🎉 KURULUM TAMAMLANDI! 🎉")
    print("\nSonraki Adımlar:")
    print(f"1. Terminalde proje klasörüne gidin: cd {API_PROJECT_NAME}")
    print(f"2. Gerekli paketleri kurun: dotnet restore")
    print(f"3. Projeyi çalıştırın: dotnet watch run")

if __name__ == "__main__":
    main()
