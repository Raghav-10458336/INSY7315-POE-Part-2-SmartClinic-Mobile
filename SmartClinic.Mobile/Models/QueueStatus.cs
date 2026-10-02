using SQLite;

namespace SmartClinic.Mobile.Models;

public class QueueStatus
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Links the queue record to the related appointment.
    [Indexed(Unique = true)]
    public int AppointmentId { get; set; }

    // Links the queue record to the patient who checked in.
    [Indexed]
    public int PatientId { get; set; }

    // Links the queue record to the doctor handling the appointment.
    [Indexed]
    public int DoctorId { get; set; }

    // Represents the patient's current position in the clinic queue.
    public int QueuePosition { get; set; }

    public DateTime? CheckedInAt { get; set; }

    public DateTime? CalledAt { get; set; }

    // Indicates whether the patient has completed check-in.
    [Indexed]
    public bool IsCheckedIn { get; set; }

    // Stores the current queue state shown to the patient.
    [Indexed]
    public string Status { get; set; } = string.Empty;

    public string DoctorName { get; set; } = string.Empty;

    public string EstimatedWaitTime { get; set; } = string.Empty;
}