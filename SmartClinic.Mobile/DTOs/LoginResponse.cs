using SmartClinic.Mobile.Models;

namespace SmartClinic.Mobile.DTOs;

public class LoginResponse
{
    public bool IsSuccess { get; set; }

    public string Message { get; set; } = string.Empty;

    // Basic account information returned after successful authentication.
    public int UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public UserRole Role { get; set; }
}