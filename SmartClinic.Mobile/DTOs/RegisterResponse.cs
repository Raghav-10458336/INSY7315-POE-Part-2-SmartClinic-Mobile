namespace SmartClinic.Mobile.DTOs;

public class RegisterResponse
{
    public bool IsSuccess { get; set; }

    // Provides confirmation or validation feedback from the API.
    public string Message { get; set; } = string.Empty;

    // Returns the identifier of the newly created account when successful.
    public int? UserId { get; set; }
}