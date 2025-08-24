// Bu sınıf, veritabanındaki 'ProviderProfiles' tablosunu temsil eder.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HizmetPinari.Api.Models;

[Table("ProviderProfiles")]
public class ProviderProfile
{
    [Key]
    public long UserID { get; set; } // Hem Primary Key hem de Foreign Key

    public string? Bio { get; set; }

    [StringLength(255)]
    public string? Expertise { get; set; }

    [StringLength(255)]
    public string? PortfolioUrl { get; set; }

    [StringLength(255)]
    public string? Location { get; set; }

    // User ile bire-bir ilişkiyi kuruyoruz.
    public User User { get; set; } = null!;
}
