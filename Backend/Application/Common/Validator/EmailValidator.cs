
using System.ComponentModel.DataAnnotations;

namespace Application.Common.Validator;

public static class EmailValidator
{
    public static void Validate(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        var emailAttribute = new EmailAddressAttribute();
        if (!emailAttribute.IsValid(email))
            throw new ArgumentException("Invalid email address.", nameof(email));
    }
}
