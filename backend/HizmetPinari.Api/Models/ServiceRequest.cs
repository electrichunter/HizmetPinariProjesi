// HizmetPinari/Models/ServiceRequest.cs

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HizmetPinari.Api.Models;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.EntityFrameworkCore.Metadata;
using IView = Microsoft.AspNetCore.Mvc.ViewEngines.IView;

namespace HizmetPinari.Models
{
    /// <summary>
    /// Veritabanındaki 'ServiceRequests' tablosunu temsil eden ana entity sınıfıdır.
    /// Müşterilerin hizmet taleplerini içerir.
    /// </summary>
    [Table("ServiceRequests")]
    public class ServiceRequest
    {
        /// <summary>
        /// Talebin birincil anahtarı (Primary Key).
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long RequestID { get; set; }

        /// <summary>
        /// Talebi oluşturan müşterinin ID'si (Foreign Key). 'Users' tablosuna işaret eder.
        /// </summary>
        [Required]
        public long CustomerID { get; set; }

        /// <summary>
        /// Talebin ait olduğu hizmet kategorisinin ID'si (Foreign Key). 'ServiceCategories' tablosuna işaret eder.
        /// </summary>
        [Required]
        public int CategoryID { get; set; }

        /// <summary>
        /// Talebin başlığı. Arayüzde listelemelerde gösterilir.
        /// </summary>
        [Required(ErrorMessage = "Başlık alanı boş bırakılamaz.")]
        [StringLength(255, ErrorMessage = "Başlık en fazla 255 karakter olabilir.")]
        public string Title { get; set; }

        /// <summary>
        /// Talebin detaylı açıklaması. Hizmet verenlerin işi anlaması için gereklidir.
        /// </summary>
        [Required(ErrorMessage = "Açıklama alanı boş bırakılamaz.")]
        public string Description { get; set; }

        /// <summary>
        /// Hizmetin verileceği konum bilgisi (örn: "İstanbul, Kadıköy").
        /// </summary>
        [StringLength(255)]
        public string Location { get; set; }

        /// <summary>
        /// Talebin mevcut durumu. Örn: OPEN_FOR_OFFERS, IN_PROGRESS, COMPLETED, CANCELLED.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "OPEN_FOR_OFFERS"; // Varsayılan değer ataması

        /// <summary>
        /// Talebin oluşturulduğu tarih ve saat.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Varsayılan değer ataması

        /// <summary>
        /// Talebin son güncellendiği tarih ve saat.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        // --- Navigation Properties (İlişkisel Veriler) ---

        /// <summary>
        /// Talebi oluşturan müşteri bilgisi. EF Core bu property üzerinden 'Users' tablosu ile join işlemi yapar.
        /// </summary>
        [ForeignKey("CustomerID")]
        public virtual User Customer { get; set; }

        /// <summary>
        /// Talebin ait olduğu kategori bilgisi. EF Core bu property üzerinden 'ServiceCategories' tablosu ile join işlemi yapar.
        /// </summary>
        [ForeignKey("CategoryID")]
        public virtual ServiceCategory Category { get; set; }

        /// <summary>
        /// Bu talebe yapılmış olan tüm tekliflerin listesi.
        /// </summary>
        public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();

        /// <summary>
        /// Bu talep için yapılmış olan tüm yorumların listesi.
        /// </summary>
        public virtual ICollection<IView> Reviews { get; set; } = new List<IView>();
    }

    public class Offer
    {
    }
}
