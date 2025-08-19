using System.ComponentModel.DataAnnotations.Schema;
namespace HizmetPinari.Api.Models;
[Table("UserRoles")]
public class UserRole {
    public long UserID { get; set; }
    public User User { get; set; } = null!;
    public int RoleID { get; set; }
    public Role Role { get; set; } = null!;
}