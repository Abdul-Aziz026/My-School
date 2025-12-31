using System;

namespace Contracts.Events;

public class SendEmailCommand
{
    public string ToMail { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;

    // optional attachment
    public string? AttachmentName { get; set; }
    public byte[]? Attachment { get; set; }
}
