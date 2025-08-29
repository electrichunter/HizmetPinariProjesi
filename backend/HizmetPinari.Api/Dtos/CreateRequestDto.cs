using System.ComponentModel.DataAnnotations;

namespace HizmetPinari.Dtos
{
    public class CreateRequestDto
    {
        [Required(ErrorMessage = "Kategori seçimi zorunludur.")]
        [Range(1, int.MaxValue, ErrorMessage = "Geçerli bir kategori ID'si girilmelidir.")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Başlık alanı zorunludur.")]
        [StringLength(255, MinimumLength = 10, ErrorMessage = "Başlık 10 ile 255 karakter arasında olmalıdır.")]
        public string Title { get; set; } = string.Empty; // CS8618 uyarısı için eklendi

        [Required(ErrorMessage = "Açıklama alanı zorunludur.")]
        [StringLength(4000, MinimumLength = 20, ErrorMessage = "Açıklama 20 ile 4000 karakter arasında olmalıdır.")]
        public string Description { get; set; } = string.Empty; // CS8618 uyarısı için eklendi

        [StringLength(255, ErrorMessage = "Konum en fazla 255 karakter olabilir.")]
        public string? Location { get; set; } // Bu alan null olabileceğinden '?' ile işaretlendi
    }
}