namespace SmartClinic.Mobile.Models;

public class Appointment
{
    public int Id { get; set; }

    // Foreign identifiers allow the API to associate the appointment
    // with the correct patient and doctor records.
    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public DateTime AppointmentDateTime { get; set; }

    // Duration is supplied by the scheduling system and can vary if required.
    public int DurationMinutes { get; set; } = 30;

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

    public string ReasonForVisit { get; set; } = string.Empty;

    // Optional patient notes supplied during the booking process.
    public string Notes { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    // These display values allow appointment cards to show useful information
    // without requiring additional lookups in the UI layer.
    public string DoctorName { get; set; } = string.Empty;

    public string DoctorSpecialisation { get; set; } = string.Empty;

    public string PatientName { get; set; } = string.Empty;
}
