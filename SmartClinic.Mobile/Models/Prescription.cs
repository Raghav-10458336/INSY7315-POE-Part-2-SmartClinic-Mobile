namespace SmartClinic.Mobile.Models;

public class Prescription
{
    public int Id { get; set; }

    public int ConsultationId { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public string DoctorName { get; set; } = string.Empty;

    public string MedicationName { get; set; } = string.Empty;

    public string Dosage { get; set; } = string.Empty;

    public string Frequency { get; set; } = string.Empty;

    public string Duration { get; set; } = string.Empty;

    // Provides additional medication instructions from the doctor.
    public string Instructions { get; set; } = string.Empty;

    public DateTime IssuedDate { get; set; }

    public DateTime? ExpiryDate { get; set; }
}