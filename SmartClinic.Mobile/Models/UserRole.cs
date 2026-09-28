namespace SmartClinic.Mobile.Models;

/// <summary>
/// Defines the authorised user roles supported by the Smart Clinic system.
/// Roles are used to determine which mobile features and information
/// are available to an authenticated user.
/// </summary>
public enum UserRole
{
    Patient,
    Doctor,
    Receptionist,
    ClinicAdmin,
    SystemAdmin
}