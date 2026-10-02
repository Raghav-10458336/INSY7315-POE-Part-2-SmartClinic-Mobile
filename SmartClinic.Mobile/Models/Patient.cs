using SQLite;

namespace SmartClinic.Mobile.Models;

public class Patient
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Links the patient profile to its local user account.
    [Indexed(Unique = true)]
    public int UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    // Indexed to support quick profile lookups by email.
    [Indexed(Unique = true)]
    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    // Emergency contact details are stored with the patient profile.
    public string EmergencyContactName { get; set; } = string.Empty;

    public string EmergencyContactNumber { get; set; } = string.Empty;

    public string MedicalAidProvider { get; set; } = string.Empty;

    public string MedicalAidNumber { get; set; } = string.Empty;

    // Calculated for display and does not require its own database column.
    [Ignore]
    public string FullName => $"{FirstName} {LastName}".Trim();
}