using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HizmetPinari.Api.Models;
[Table("Roles")]
public class Role {
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)] public int RoleID { get; set; }
    [Required, StringLength(50)] public string RoleName { get; set; } = string.Empty;
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}