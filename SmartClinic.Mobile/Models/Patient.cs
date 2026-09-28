namespace SmartClinic.Mobile.Models;

public class Patient
{
    public int Id { get; set; }

    // Links the patient profile to the authenticated user account.
    public int UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public string Gender { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    // Emergency contact information is kept with the patient profile
    // so it can be displayed quickly when required.
    public string EmergencyContactName { get; set; } = string.Empty;

    public string EmergencyContactNumber { get; set; } = string.Empty;

    public string MedicalAidProvider { get; set; } = string.Empty;

    public string MedicalAidNumber { get; set; } = string.Empty;

    // Useful for displaying the patient's name consistently across screens.
    public string FullName => $"{FirstName} {LastName}".Trim();
}