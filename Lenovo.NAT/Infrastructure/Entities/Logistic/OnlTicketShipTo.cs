using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lenovo.NAT.Infrastructure.Entities.Logistic
{
    public class OnlTicketShipTo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OnlTicketSoldToId { get; set; }

        [StringLength(18)]
        public string? CNPJ { get; set; }

        [StringLength(255)]
        public string? Endereco { get; set; }

        [StringLength(100)]
        public string? Bairro { get; set; }

        [StringLength(100)]
        public string? Municipio { get; set; }

        [StringLength(10)]
        public string? CEP { get; set; }

        [StringLength(2)]
        public string? UF { get; set; }

        [StringLength(100)]
        public string? SAPOrderNumber { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        [StringLength(100)]
        public string CreatedBy { get; set; } = string.Empty;

        // Navigation Properties
        [ForeignKey("OnlTicketSoldToId")]
        public virtual OnlTicketSoldTo OnlTicketSoldTo { get; set; } = null!;

        public virtual ICollection<OnlTicketOrderItem> OrderItems { get; set; } = new List<OnlTicketOrderItem>();
    }
}
