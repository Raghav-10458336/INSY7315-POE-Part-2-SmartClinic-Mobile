using SQLite;

namespace SmartClinic.Mobile.Models;

public class Consultation
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Links the consultation to the appointment it resulted from.
    [Indexed(Unique = true)]
    public int AppointmentId { get; set; }

    // Links the consultation to the patient.
    [Indexed]
    public int PatientId { get; set; }

    // Links the consultation to the doctor who conducted it.
    [Indexed]
    public int DoctorId { get; set; }

    // Indexed to support chronological consultation history.
    [Indexed]
    public DateTime ConsultationDate { get; set; }

    public string DoctorName { get; set; } = string.Empty;

    public string Diagnosis { get; set; } = string.Empty;

    public string Treatment { get; set; } = string.Empty;

    // Patient-facing summary of the completed consultation.
    public string Summary { get; set; } = string.Empty;

    // Stores any instructions provided for continued care.
    public string FollowUpInstructions { get; set; } = string.Empty;

    public DateTime? FollowUpDate { get; set; }
}