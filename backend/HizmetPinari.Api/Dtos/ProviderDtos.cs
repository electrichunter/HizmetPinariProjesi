// Bu dosya, Hizmet Veren ile ilgili tüm DTO'ları içerir.
using System.ComponentModel.DataAnnotations;

namespace HizmetPinari.Api.Dtos;

// Yeni bir Hizmet Veren kaydı için kullanılacak DTO
public class ProviderRegisterDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;
    [Required]
    public string FirstName { get; set; } = string.Empty;
    [Required]
    public string LastName { get; set; } = string.Empty;
}

// Profil güncellemek için kullanılacak DTO
public class UpdateProviderProfileDto
{
    public string? Bio { get; set; }
    [StringLength(255)]
    public string? Expertise { get; set; }
    [StringLength(255)]
    public string? PortfolioUrl { get; set; }
    [StringLength(255)]
    public string? Location { get; set; }
}

// Bir profil görüntülerken kullanıcıya dönecek DTO
public class ProviderProfileDto
{
    public long UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? Expertise { get; set; }
    public string? PortfolioUrl { get; set; }
    public string? Location { get; set; }
}
