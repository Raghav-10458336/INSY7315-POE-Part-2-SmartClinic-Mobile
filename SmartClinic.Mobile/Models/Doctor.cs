namespace SmartClinic.Mobile.Models;

public class Doctor
{
    public int Id { get; set; }

    // Links the doctor profile to the authenticated user account.
    public int UserId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Specialisation { get; set; } = string.Empty;

    // Professional registration number used to identify the practitioner.
    public string RegistrationNumber { get; set; } = string.Empty;

    public string Qualification { get; set; } = string.Empty;

    public string Biography { get; set; } = string.Empty;

    // Allows unavailable doctors to be excluded from appointment booking.
    public bool IsAvailable { get; set; } = true;

    public string FullName => $"Dr {FirstName} {LastName}".Trim();
}
