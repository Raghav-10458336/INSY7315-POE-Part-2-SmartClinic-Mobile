namespace SmartClinic.Mobile.DTOs;

public class RegisterResponse
{
    public bool IsSuccess { get; set; }

    // Provides confirmation or validation feedback after registration.
    public string Message { get; set; } = string.Empty;

    // Identifier assigned to the newly created account.
    public int? UserId { get; set; }
}