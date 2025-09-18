using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lenovo.NAT.Infrastructure.Entities.Logistic
{
    public class OnlTicketComment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OnlTicketId { get; set; }

        [Required]
        public string Comment { get; set; } = string.Empty;

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
