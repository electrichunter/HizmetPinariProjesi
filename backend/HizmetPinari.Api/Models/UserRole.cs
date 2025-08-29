
using System.ComponentModel.DataAnnotations.Schema;

namespace HizmetPinari.Api.Models
{
    [Table("UserRoles")]
    public class UserRole
    {
        public long UserID { get; set; }
        public int RoleID { get; set; }

        // Navigation Properties
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}
