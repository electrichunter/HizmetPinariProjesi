// Bu dosya, kategori işlemleri için kullanılacak tüm DTO'ları içerir.
using System.ComponentModel.DataAnnotations;

namespace HizmetPinari.Api.Dtos;

// Yeni bir kategori oluşturmak için kullanılacak DTO
public class CreateCategoryDto
{
    [Required(ErrorMessage = "Kategori adı boş olamaz.")]
    [StringLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    public string? Description { get; set; }
    
    public int? ParentCategoryID { get; set; }
}

// Mevcut bir kategoriyi güncellemek için kullanılacak DTO
public class UpdateCategoryDto
{
    [Required(ErrorMessage = "Kategori adı boş olamaz.")]
    [StringLength(100)]
    public string CategoryName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int? ParentCategoryID { get; set; }

    public bool IsActive { get; set; } = true;
}
