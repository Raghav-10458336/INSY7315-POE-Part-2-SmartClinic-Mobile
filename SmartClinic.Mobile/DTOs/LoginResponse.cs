using SmartClinic.Mobile.Models;

namespace SmartClinic.Mobile.DTOs;

public class LoginResponse
{
    // Indicates whether authentication was successful.
    public bool IsSuccess { get; set; }

    // Contains a user-friendly response from the API.
    public string Message { get; set; } = string.Empty;

    // Authentication token returned after a successful login.
    public string Token { get; set; } = string.Empty;

    // Basic authenticated user information used by the mobile app.
    public int UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public UserRole Role { get; set; }
}