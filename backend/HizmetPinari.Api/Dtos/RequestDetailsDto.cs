
// HizmetPinari/Dtos/RequestDetailsDto.cs

using System;

namespace HizmetPinari.Dtos
{
    /// <summary>
    /// API'den client'a hizmet talebi detaylarını dönerken kullanılacak DTO.
    /// Veritabanı modelini doğrudan dışarı açmak yerine, sadece arayüzün ihtiyaç duyduğu
    /// ve gösterilmesi güvenli olan verileri içerir.
    /// </summary>
    public class RequestDetailsDto
    {
        public long RequestID { get; set; }
        public long CustomerID { get; set; }
        public string CustomerFullName { get; set; } // Geliştirme: Müşteri adını ve soyadını birleştirerek sunar.
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public int OfferCount { get; set; } // Geliştirme: Talebe gelen toplam teklif sayısını gösterir.
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
