using SQLite;

namespace SmartClinic.Mobile.Models;

public class Prescription
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    // Links the prescription to the consultation where it was issued.
    [Indexed]
    public int ConsultationId { get; set; }

    // Links the prescription to the patient receiving the medication.
    [Indexed]
    public int PatientId { get; set; }

    // Links the prescription to the prescribing doctor.
    [Indexed]
    public int DoctorId { get; set; }

    public string DoctorName { get; set; } = string.Empty;

    public string MedicationName { get; set; } = string.Empty;

    public string Dosage { get; set; } = string.Empty;

    public string Frequency { get; set; } = string.Empty;

    public string Duration { get; set; } = string.Empty;

    // Stores any additional instructions provided with the prescription.
    public string Instructions { get; set; } = string.Empty;

    // Indexed to support chronological prescription history.
    [Indexed]
    public DateTime IssuedDate { get; set; }

    public DateTime? ExpiryDate { get; set; }
}