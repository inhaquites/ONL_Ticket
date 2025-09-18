using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lenovo.NAT.Infrastructure.Entities.Logistic
{
    public class OnlTicket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string LogNumber { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        [StringLength(100)]
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime? UpdatedDate { get; set; }

        [StringLength(100)]
        public string? UpdatedBy { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = string.Empty;

        [StringLength(255)]
        public string? EmailFrom { get; set; }

        [StringLength(100)]
        public string? AssignedOperator { get; set; }

        [StringLength(100)]
        public string? PONumber { get; set; }

        [StringLength(50)]
        public string? OrderType { get; set; }

        [StringLength(50)]
        public string? OrderStatus { get; set; }

        [StringLength(50)]
        public string? NFType { get; set; }

        [StringLength(255)]
        public string? CustomerName { get; set; }

        [StringLength(50)]
        public string? DMU { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        public Guid? CountryId { get; set; }

        [StringLength(50)]
        public string? BU { get; set; }

        public DateTime? PODate { get; set; }

        [StringLength(50)]
        public string? Segment { get; set; }

        public int? SegmentId { get; set; }

        [StringLength(50)]
        public string? BillAhead { get; set; }

        [StringLength(100)]
        public string? ISRName { get; set; }

        [StringLength(100)]
        public string? Region { get; set; }

        [StringLength(2)]
        public string? UF { get; set; }

        [StringLength(50)]
        public string? ReplacementType { get; set; }

        public string? Comentarios { get; set; }

        [StringLength(100)]
        public string? OrderId { get; set; }

        // Navigation Properties
        [ForeignKey("CountryId")]
        public virtual Admin.Country? CountryNavigation { get; set; }

        [ForeignKey("SegmentId")]
        public virtual CustomerSegment? SegmentNavigation { get; set; }

        public virtual ICollection<OnlTicketAttachment> Attachments { get; set; } = new List<OnlTicketAttachment>();
        public virtual ICollection<OnlTicketSoldTo> SoldToAddresses { get; set; } = new List<OnlTicketSoldTo>();
        public virtual ICollection<OnlTicketSAPOrder> SAPOrders { get; set; } = new List<OnlTicketSAPOrder>();
        public virtual ICollection<OnlTicketComment> Comments { get; set; } = new List<OnlTicketComment>();
    }
}
