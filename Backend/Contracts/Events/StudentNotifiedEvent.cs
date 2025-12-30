
namespace Contracts.Events;

public class StudentNotifiedEvent
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}
