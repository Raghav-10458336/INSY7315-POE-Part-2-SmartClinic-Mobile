using SmartClinic.Mobile.Data;
using SmartClinic.Mobile.Models;

namespace SmartClinic.Mobile.Services;

public class ClinicalService : IClinicalService
{
    private readonly SmartClinicDatabase _smartClinicDatabase;
    private readonly IAuthenticationService _authenticationService;

    public ClinicalService(
        SmartClinicDatabase smartClinicDatabase,
        IAuthenticationService authenticationService)
    {
        _smartClinicDatabase = smartClinicDatabase;
        _authenticationService = authenticationService;
    }

    public async Task<List<Consultation>> GetConsultationsAsync()
    {
        try
        {
            var patient = await GetCurrentPatientAsync();

            if (patient is null)
            {
                return [];
            }

            var database = await _smartClinicDatabase.GetConnectionAsync();

            // Return only consultation records belonging to the signed-in patient.
            return await database.Table<Consultation>()
                .Where(consultation => consultation.PatientId == patient.Id)
                .OrderByDescending(consultation => consultation.ConsultationDate)
                .ToListAsync();
        }
        catch (Exception)
        {
            return [];
        }
    }

    public async Task<Consultation?> GetConsultationAsync(
        int consultationId)
    {
        try
        {
            if (consultationId <= 0)
            {
                return null;
            }

            var patient = await GetCurrentPatientAsync();

            if (patient is null)
            {
                return null;
            }

            var database = await _smartClinicDatabase.GetConnectionAsync();

            // Patient ownership is verified before clinical details are exposed.
            return await database.Table<Consultation>()
                .Where(consultation =>
                    consultation.Id == consultationId &&
                    consultation.PatientId == patient.Id)
                .FirstOrDefaultAsync();
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<List<Prescription>> GetPrescriptionsAsync()
    {
        try
        {
            var patient = await GetCurrentPatientAsync();

            if (patient is null)
            {
                return [];
            }

            var database = await _smartClinicDatabase.GetConnectionAsync();

            // Return the patient's prescription history newest first.
            return await database.Table<Prescription>()
                .Where(prescription => prescription.PatientId == patient.Id)
                .OrderByDescending(prescription => prescription.IssuedDate)
                .ToListAsync();
        }
        catch (Exception)
        {
            return [];
        }
    }

    public async Task<List<Prescription>> GetConsultationPrescriptionsAsync(
        int consultationId)
    {
        try
        {
            if (consultationId <= 0)
            {
                return [];
            }

            var patient = await GetCurrentPatientAsync();

            if (patient is null)
            {
                return [];
            }

            var database = await _smartClinicDatabase.GetConnectionAsync();

            // Verify that the consultation belongs to the signed-in patient.
            var consultation = await database.Table<Consultation>()
                .Where(existing =>
                    existing.Id == consultationId &&
                    existing.PatientId == patient.Id)
                .FirstOrDefaultAsync();

            if (consultation is null)
            {
                return [];
            }

            return await database.Table<Prescription>()
                .Where(prescription =>
                    prescription.ConsultationId == consultation.Id &&
                    prescription.PatientId == patient.Id)
                .OrderByDescending(prescription => prescription.IssuedDate)
                .ToListAsync();
        }
        catch (Exception)
        {
            return [];
        }
    }

    private async Task<Patient?> GetCurrentPatientAsync()
    {
        var authenticationState =
            await _authenticationService.GetAuthenticationStateAsync();

        if (!authenticationState.IsAuthenticated ||
            authenticationState.UserId is null)
        {
            return null;
        }

        var database = await _smartClinicDatabase.GetConnectionAsync();
        var userId = authenticationState.UserId.Value;

        return await database.Table<Patient>()
            .Where(patient => patient.UserId == userId)
            .FirstOrDefaultAsync();
    }
}