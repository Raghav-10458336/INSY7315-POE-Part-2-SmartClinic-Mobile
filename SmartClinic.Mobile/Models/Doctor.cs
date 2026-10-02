using SQLite;

namespace SmartClinic.Mobile.Models;

public class Doctor
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Links the doctor profile to its local user account when applicable.
    [Indexed]
    public int UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    // Indexed to support quick doctor lookups by email.
    [Indexed(Unique = true)]
    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    // Used when filtering doctors during appointment booking.
    [Indexed]
    public string Specialisation { get; set; } = string.Empty;

    // Uniquely identifies the practitioner within the local database.
    [Indexed(Unique = true)]
    public string RegistrationNumber { get; set; } = string.Empty;

    public string Qualification { get; set; } = string.Empty;

    public string Biography { get; set; } = string.Empty;

    // Unavailable doctors are excluded from new appointment bookings.
    public bool IsAvailable { get; set; } = true;

    // Calculated for display and does not require its own database column.
    [Ignore]
    public string FullName => $"Dr {FirstName} {LastName}".Trim();
}