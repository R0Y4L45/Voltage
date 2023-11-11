using MimeKit;

namespace Voltage.Entities.Models.HelperModels;

public class E_Message
{
    public List<MailboxAddress> To { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
    public E_Message(IEnumerable<string> to, string subject, string content)
    {
        To = new List<MailboxAddress>();
        To.AddRange(to.Select(x => new MailboxAddress("email", x)));
        Subject = subject;
        Content = content;
    }
}
