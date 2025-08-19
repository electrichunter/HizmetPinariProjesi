// Bu sınıf, veritabanındaki 'ServiceCategories' tablosunu temsil eder.
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HizmetPinari.Api.Models;

[Table("ServiceCategories")]
public class ServiceCategory
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CategoryID { get; set; }

    // Bir kategorinin başka bir kategori altında olmasını sağlar (örn: Tadilat -> Boya Badana)
    public int? ParentCategoryID { get; set; }

    [Required, StringLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}
