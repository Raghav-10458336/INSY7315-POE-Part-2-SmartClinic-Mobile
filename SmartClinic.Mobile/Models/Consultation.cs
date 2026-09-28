namespace SmartClinic.Mobile.Models;

public class Consultation
{
    public int Id { get; set; }

    public int AppointmentId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public DateTime ConsultationDate { get; set; }

    public string DoctorName { get; set; } = string.Empty;

    public string Diagnosis { get; set; } = string.Empty;

    public string Treatment { get; set; } = string.Empty;

    // Stores the consultation summary that can later be viewed by the patient.
    public string Summary { get; set; } = string.Empty;

    // Records any follow-up instructions provided after the consultation.
    public string FollowUpInstructions { get; set; } = string.Empty;

    public DateTime? FollowUpDate { get; set; }
}
