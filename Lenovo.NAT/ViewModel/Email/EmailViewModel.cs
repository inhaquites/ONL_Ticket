using System.ComponentModel.DataAnnotations;

namespace Lenovo.NAT.ViewModel.Email
{
    public class EmailViewModel
    {
        public string RoleId { get; set; }
        public string From { get; set; }
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string To { get; set; }
        public string Cc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<EmailAttachmentsViewModel> Attachments { get; set; }
    }

    public class EmailAttachmentsViewModel
    {
        public byte[]? AttachedFile { get; set; }
        public string? AttachedFileName { get; set; }
    }
}