namespace SmartClinic.Mobile.Models;

public class AuthenticationState
{
    // Tracks whether a user currently has an authenticated session.
    public bool IsAuthenticated { get; set; }

    public int? UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public UserRole? Role { get; set; }

    // Provides a consistent display name for the signed-in user.
    public string FullName => $"{FirstName} {LastName}".Trim();
}
