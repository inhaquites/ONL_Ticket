namespace Lenovo.NAT.Infrastructure.Entities.Logistic
{
    public class OrderAttachment
    {
        public int Id { get; set; }
        public long IdOrderNotLoaded { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string PONumber { get; set; }
        public byte[] Attachment { get; set; }
        public string AttachemntFileName { get; set; }
        public string Comments { get; set; }
        public string Description { get; set; }
        
        // Campos adicionados para compatibilidade com OnlTicketAttachment
        public string? FileExtension { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
