using SQLite;

namespace SmartClinic.Mobile.Models;

public class User
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    // Email is the unique identifier used when signing in.
    [Indexed(Unique = true)]
    public string Email { get; set; } = string.Empty;

    // Stores only the hashed password used for local authentication.
    public string PasswordHash { get; set; } = string.Empty;

    // Controls which areas of the app the account can access.
    public UserRole Role { get; set; }

    // Inactive accounts cannot sign in to the application.
    public bool IsActive { get; set; } = true;

    // Provides a consistent display name throughout the app.
    [Ignore]
    public string FullName => $"{FirstName} {LastName}".Trim();
}