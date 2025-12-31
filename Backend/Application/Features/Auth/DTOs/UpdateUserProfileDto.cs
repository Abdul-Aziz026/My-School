namespace Application.Features.Auth.DTOs;

public class UpdateUserProfileDto
{
    public string UserName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string ProfilePicture { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}
