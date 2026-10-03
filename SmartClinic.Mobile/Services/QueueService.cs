using SmartClinic.Mobile.Data;
using SmartClinic.Mobile.Models;

namespace SmartClinic.Mobile.Services;

public class QueueService : IQueueService
{
    private readonly SmartClinicDatabase _smartClinicDatabase;
    private readonly IAuthenticationService _authenticationService;

    public QueueService(
        SmartClinicDatabase smartClinicDatabase,
        IAuthenticationService authenticationService)
    {
        _smartClinicDatabase = smartClinicDatabase;
        _authenticationService = authenticationService;
    }

    public async Task<QueueStatus?> CheckInAsync(int appointmentId)
    {
        try
        {
            if (appointmentId <= 0)
            {
                return null;
            }

            var patient = await GetCurrentPatientAsync();

            if (patient is null)
            {
                return null;
            }

            var database = await _smartClinicDatabase.GetConnectionAsync();

            // Only the authenticated patient can access this appointment.
            var appointment = await database.Table<Appointment>()
                .Where(existing =>
                    existing.Id == appointmentId &&
                    existing.PatientId == patient.Id)
                .FirstOrDefaultAsync();

            if (appointment is null)
            {
                return null;
            }

            // If already checked in, return the existing queue record.
            if (appointment.Status == AppointmentStatus.CheckedIn)
            {
                return await database.Table<QueueStatus>()
                    .Where(queue =>
                        queue.AppointmentId == appointment.Id &&
                        queue.PatientId == patient.Id)
                    .FirstOrDefaultAsync();
            }

            // Only scheduled appointments are eligible for a new check-in.
            if (appointment.Status != AppointmentStatus.Scheduled)
            {
                return null;
            }

            var now = DateTime.Now;

            // Check-in opens 60 minutes before the appointment and
            // remains available until 30 minutes after the scheduled time.
            var checkInOpens =
                appointment.AppointmentDateTime.AddMinutes(-60);

            var checkInCloses =
                appointment.AppointmentDateTime.AddMinutes(30);

            if (now < checkInOpens || now > checkInCloses)
            {
                return null;
            }

            // An appointment can only have one queue record.
            var existingQueueStatus = await database.Table<QueueStatus>()
                .Where(queue =>
                    queue.AppointmentId == appointment.Id &&
                    queue.PatientId == patient.Id)
                .FirstOrDefaultAsync();

            if (existingQueueStatus is not null)
            {
                if (!existingQueueStatus.IsCheckedIn)
                {
                    return null;
                }

                // Keep the appointment lifecycle consistent with the queue.
                appointment.Status = AppointmentStatus.CheckedIn;
                appointment.UpdatedAt = now;

                await database.UpdateAsync(appointment);

                return existingQueueStatus;
            }

            var doctor = await database.FindAsync<Doctor>(
                appointment.DoctorId);

            if (doctor is null)
            {
                return null;
            }

            var queuePosition = await GetNextQueuePositionAsync(
                appointment.DoctorId);

            var queueStatus = new QueueStatus
            {
                AppointmentId = appointment.Id,
                PatientId = patient.Id,
                DoctorId = appointment.DoctorId,
                QueuePosition = queuePosition,
                CheckedInAt = now,
                IsCheckedIn = true,
                Status = "Waiting",
                DoctorName = doctor.FullName,
                EstimatedWaitTime =
                    CalculateEstimatedWaitTime(queuePosition)
            };

            await database.InsertAsync(queueStatus);

            // Move the appointment into the checked-in lifecycle state.
            appointment.Status = AppointmentStatus.CheckedIn;
            appointment.UpdatedAt = now;

            await database.UpdateAsync(appointment);

            return queueStatus;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<QueueStatus?> GetQueueStatusAsync(int appointmentId)
    {
        try
        {
            if (appointmentId <= 0)
            {
                return null;
            }

            var patient = await GetCurrentPatientAsync();

            if (patient is null)
            {
                return null;
            }

            var database = await _smartClinicDatabase.GetConnectionAsync();

            // Verify ownership before exposing queue information.
            var appointment = await database.Table<Appointment>()
                .Where(existing =>
                    existing.Id == appointmentId &&
                    existing.PatientId == patient.Id)
                .FirstOrDefaultAsync();

            if (appointment is null)
            {
                return null;
            }

            var queueStatus = await database.Table<QueueStatus>()
                .Where(queue =>
                    queue.AppointmentId == appointment.Id &&
                    queue.PatientId == patient.Id)
                .FirstOrDefaultAsync();

            if (queueStatus is null)
            {
                return null;
            }

            // Repair older persisted queue data where the patient was
            // checked in before appointment lifecycle syncing was added.
            if (queueStatus.IsCheckedIn &&
                appointment.Status == AppointmentStatus.Scheduled)
            {
                appointment.Status = AppointmentStatus.CheckedIn;
                appointment.UpdatedAt = DateTime.Now;

                await database.UpdateAsync(appointment);
            }

            return queueStatus;
        }
        catch (Exception)
        {
            return null;
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

    private async Task<int> GetNextQueuePositionAsync(int doctorId)
    {
        var database = await _smartClinicDatabase.GetConnectionAsync();

        // Count patients still waiting for this doctor.
        var waitingPatients = await database.Table<QueueStatus>()
            .Where(queue =>
                queue.DoctorId == doctorId &&
                queue.IsCheckedIn &&
                queue.Status == "Waiting")
            .CountAsync();

        return waitingPatients + 1;
    }

    private static string CalculateEstimatedWaitTime(int queuePosition)
    {
        if (queuePosition <= 1)
        {
            return "Approximately 5 minutes";
        }

        // Simple local estimate of 15 minutes per patient ahead.
        var estimatedMinutes =
            ((queuePosition - 1) * 15) + 5;

        return $"Approximately {estimatedMinutes} minutes";
    }
}