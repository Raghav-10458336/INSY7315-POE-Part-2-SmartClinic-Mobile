namespace SmartClinic.Mobile.Models;

public class QueueStatus
{
    public int Id { get; set; }

    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    // Position currently assigned to the patient in the clinic queue.
    public int QueuePosition { get; set; }

    public DateTime? CheckedInAt { get; set; }

    public DateTime? CalledAt { get; set; }

    public bool IsCheckedIn { get; set; }

    // Provides a simple status for displaying queue progress to the patient.
    public string Status { get; set; } = string.Empty;

    public string DoctorName { get; set; } = string.Empty;

    public string EstimatedWaitTime { get; set; } = string.Empty;
}