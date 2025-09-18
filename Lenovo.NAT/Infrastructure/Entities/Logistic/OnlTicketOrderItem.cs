using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lenovo.NAT.Infrastructure.Entities.Logistic
{
    public class OnlTicketOrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OnlTicketShipToId { get; set; }

        [StringLength(100)]
        public string? BidContractNumber { get; set; }

        [StringLength(100)]
        public string? PartNumber { get; set; }

        [StringLength(500)]
        public string? PartNumberDescription { get; set; }

        public int? Qty { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? UnityNetPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? UnitGrossPrice { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        [StringLength(100)]
        public string CreatedBy { get; set; } = string.Empty;

        // Navigation Properties
        [ForeignKey("OnlTicketShipToId")]
        public virtual OnlTicketShipTo OnlTicketShipTo { get; set; } = null!;
    }
}
