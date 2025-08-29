
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HizmetPinari.Api.Models
{
    [Table("ServiceRequests")]
    public class ServiceRequest
    {
        [Key]
        public long RequestID { get; set; }
        public long CustomerID { get; set; }
        public int CategoryID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string Status { get; set; } = "OPEN_FOR_OFFERS";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("CustomerID")]
        public virtual User Customer { get; set; }
    }
}
