namespace SmartClinic.Mobile.Models;

/// <summary>
/// Represents the core identity information for an authenticated
/// Smart Clinic user.
/// </summary>
public class User
{
    /// <summary>
    /// Unique identifier assigned to the user by the backend system.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User's first name as stored in their profile.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's surname as stored in their profile.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Email address associated with the user's account.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Role assigned to the user for role-based access control.
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Indicates whether the account is currently active.
    /// Disabled accounts should not be permitted to access protected features.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Returns the user's display name without duplicating formatting
    /// logic throughout the application.
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();
}
