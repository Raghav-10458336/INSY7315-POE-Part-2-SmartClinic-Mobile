using SmartClinic.Mobile.Models;

namespace SmartClinic.Mobile.Services;

public interface IPatientService
{
    // Retrieves the profile linked to the currently authenticated patient.
    Task<Patient?> GetCurrentPatientAsync();

    // Updates the authenticated patient's editable profile information.
    Task<bool> UpdatePatientAsync(Patient patient);
}