namespace SmartClinic.Mobile.Models;

// Represents the lifecycle of an appointment within the clinic.
public enum AppointmentStatus
{
    Scheduled,
    Confirmed,
    CheckedIn,
    InQueue,
    InConsultation,
    Completed,
    Cancelled,
    NoShow
}