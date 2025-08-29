
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HizmetPinari.Api.Models
{
    [Table("Offers")]
    public class Offer
    {
        [Key]
        public long OfferID { get; set; }
        public long RequestID { get; set; }
        public long ProviderID { get; set; }
        public decimal Price { get; set; }
        public string? Message { get; set; }
        public string Status { get; set; } = "PENDING";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ProviderID")]
        public virtual User ServiceProvider { get; set; }
    }
}
