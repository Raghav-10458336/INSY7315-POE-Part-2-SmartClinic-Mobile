using SQLite;

namespace SmartClinic.Mobile.Models;

public class Appointment
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Links the appointment to the patient who made the booking.
    [Indexed]
    public int PatientId { get; set; }

    // Links the appointment to the selected doctor.
    [Indexed]
    public int DoctorId { get; set; }

    // Indexed because appointments are frequently filtered and ordered by date.
    [Indexed]
    public DateTime AppointmentDateTime { get; set; }

    public int DurationMinutes { get; set; } = 30;

    // Tracks the appointment throughout its lifecycle.
    [Indexed]
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

    public string ReasonForVisit { get; set; } = string.Empty;

    public string Notes { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // Display values keep appointment cards simple while retaining ID relationships.
    public string DoctorName { get; set; } = string.Empty;

    public string DoctorSpecialisation { get; set; } = string.Empty;

    public string PatientName { get; set; } = string.Empty;

    // UI helpers are calculated from the appointment lifecycle and are not stored.
    [Ignore]
    public bool CanManage =>
        Status == AppointmentStatus.Scheduled ||
        Status == AppointmentStatus.Confirmed;

    [Ignore]
    public bool CanAccessCheckIn =>
        Status == AppointmentStatus.Scheduled ||
        Status == AppointmentStatus.CheckedIn ||
        Status == AppointmentStatus.InQueue;

    [Ignore]
    public bool IsQueueActive =>
        Status == AppointmentStatus.CheckedIn ||
        Status == AppointmentStatus.InQueue;

    [Ignore]
    public string QueueActionText =>
        IsQueueActive
            ? "View Queue Status"
            : "Check In & View Queue";
}