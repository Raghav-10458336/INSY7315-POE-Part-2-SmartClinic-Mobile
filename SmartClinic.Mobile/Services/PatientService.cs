using SmartClinic.Mobile.Data;
using SmartClinic.Mobile.Models;

namespace SmartClinic.Mobile.Services;

public class PatientService : IPatientService
{
    private readonly SmartClinicDatabase _smartClinicDatabase;
    private readonly IAuthenticationService _authenticationService;

    public PatientService(
        SmartClinicDatabase smartClinicDatabase,
        IAuthenticationService authenticationService)
    {
        _smartClinicDatabase = smartClinicDatabase;
        _authenticationService = authenticationService;
    }

    public async Task<Patient?> GetCurrentPatientAsync()
    {
        try
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
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> UpdatePatientAsync(Patient patient)
    {
        try
        {
            var authenticationState =
                await _authenticationService.GetAuthenticationStateAsync();

            if (!authenticationState.IsAuthenticated ||
                authenticationState.UserId is null)
            {
                return false;
            }

            var database = await _smartClinicDatabase.GetConnectionAsync();
            var userId = authenticationState.UserId.Value;

            // Always retrieve the stored profile before applying editable changes.
            var existingPatient = await database.Table<Patient>()
                .Where(existing => existing.UserId == userId)
                .FirstOrDefaultAsync();

            if (existingPatient is null || existingPatient.Id != patient.Id)
            {
                return false;
            }

            var normalizedEmail = patient.Email.Trim().ToLowerInvariant();

            // Prevent another account from using the same patient email.
            var duplicatePatient = await database.Table<Patient>()
                .Where(existing =>
                    existing.Email == normalizedEmail &&
                    existing.Id != existingPatient.Id)
                .FirstOrDefaultAsync();

            if (duplicatePatient is not null)
            {
                return false;
            }

            existingPatient.FirstName = patient.FirstName.Trim();
            existingPatient.LastName = patient.LastName.Trim();
            existingPatient.Email = normalizedEmail;
            existingPatient.PhoneNumber = patient.PhoneNumber.Trim();
            existingPatient.DateOfBirth = patient.DateOfBirth;
            existingPatient.Gender = patient.Gender.Trim();
            existingPatient.Address = patient.Address.Trim();
            existingPatient.EmergencyContactName =
                patient.EmergencyContactName.Trim();
            existingPatient.EmergencyContactNumber =
                patient.EmergencyContactNumber.Trim();
            existingPatient.MedicalAidProvider =
                patient.MedicalAidProvider.Trim();
            existingPatient.MedicalAidNumber =
                patient.MedicalAidNumber.Trim();

            var updatedRows = await database.UpdateAsync(existingPatient);

            if (updatedRows <= 0)
            {
                return false;
            }

            // Keep account identity information consistent with the patient profile.
            var user = await database.FindAsync<User>(userId);

            if (user is not null)
            {
                var duplicateUser = await database.Table<User>()
                    .Where(existing =>
                        existing.Email == normalizedEmail &&
                        existing.Id != user.Id)
                    .FirstOrDefaultAsync();

                if (duplicateUser is not null)
                {
                    return false;
                }

                user.FirstName = existingPatient.FirstName;
                user.LastName = existingPatient.LastName;
                user.Email = existingPatient.Email;

                await database.UpdateAsync(user);
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}