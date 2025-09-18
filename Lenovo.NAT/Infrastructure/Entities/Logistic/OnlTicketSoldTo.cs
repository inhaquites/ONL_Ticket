using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lenovo.NAT.Infrastructure.Entities.Logistic
{
    public class OnlTicketSoldTo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OnlTicketId { get; set; }

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

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        [StringLength(100)]
        public string CreatedBy { get; set; } = string.Empty;

        // Navigation Properties
        [ForeignKey("OnlTicketId")]
        public virtual OnlTicket OnlTicket { get; set; } = null!;

        public virtual ICollection<OnlTicketShipTo> ShipToAddresses { get; set; } = new List<OnlTicketShipTo>();
    }
}
