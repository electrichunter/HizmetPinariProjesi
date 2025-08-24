using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HizmetPinari.Api.Models;
[Table("Users")]
public class User {
    internal ProviderProfile ProviderProfil;

    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] public long UserID { get; set; }
    [Required, EmailAddress, StringLength(255)] public string Email { get; set; } = string.Empty;
    [Required, StringLength(255)] public string PasswordHash { get; set; } = string.Empty;
    [Required, StringLength(100)] public string FirstName { get; set; } = string.Empty;
    [Required, StringLength(100)] public string LastName { get; set; } = string.Empty;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}