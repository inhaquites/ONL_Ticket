using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lenovo.NAT.Infrastructure.Entities.Logistic
{
    public class OnlTicketSAPOrder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OnlTicketId { get; set; }

        [StringLength(100)]
        public string? SAPOrderNumber { get; set; }

        [StringLength(100)]
        public string? DeliveryNumber { get; set; }

        [StringLength(100)]
        public string? InvoiceNumber { get; set; }

        [StringLength(100)]
        public string? NFNumber { get; set; }

        public DateTime? NFDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? NetAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalOrderAmount { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        [StringLength(100)]
        public string CreatedBy { get; set; } = string.Empty;

        // Navigation Properties
        [ForeignKey("OnlTicketId")]
        public virtual OnlTicket OnlTicket { get; set; } = null!;
    }
}
