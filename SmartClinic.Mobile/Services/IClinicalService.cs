using SmartClinic.Mobile.Models;

namespace SmartClinic.Mobile.Services;

public interface IClinicalService
{
    // Retrieves the authenticated patient's consultation history.
    Task<List<Consultation>> GetConsultationsAsync();

    // Retrieves the full details of a selected consultation.
    Task<Consultation?> GetConsultationAsync(int consultationId);

    // Retrieves all prescriptions belonging to the authenticated patient.
    Task<List<Prescription>> GetPrescriptionsAsync();

    // Retrieves prescriptions issued during a selected consultation.
    Task<List<Prescription>> GetConsultationPrescriptionsAsync(
        int consultationId);
}