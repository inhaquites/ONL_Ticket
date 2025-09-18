using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lenovo.NAT.Infrastructure.Entities.Logistic
{
    public class OnlTicketAttachment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OnlTicketId { get; set; }

        [StringLength(100)]
        public string? CustomerPO { get; set; }

        [StringLength(500)]
        public string? Descricao { get; set; }

        public string? Comentarios { get; set; }

        [StringLength(500)]
        public string? FilePath { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public long FileSize { get; set; }

        [Required]
        [StringLength(100)]
        public string ContentType { get; set; } = string.Empty;

        [StringLength(10)]
        public string? FileExtension { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        [StringLength(100)]
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime? UpdatedDate { get; set; }

        [StringLength(100)]
        public string? UpdatedBy { get; set; }

        // Navigation Properties
        [ForeignKey("OnlTicketId")]
        public virtual OnlTicket OnlTicket { get; set; } = null!;
    }
}
