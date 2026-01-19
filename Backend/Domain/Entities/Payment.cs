namespace Domain.Entities;

public class Payment : BaseEntity
{
    public string StudentId { get; set; } = string.Empty;
    public string EnrollmentId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public string PaymentMethod { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; }

    public string GetId() => Id;
}


public enum PaymentStatus
{
    Paid,
    Pending,
    Failed
}
