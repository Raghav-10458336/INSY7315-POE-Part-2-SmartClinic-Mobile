namespace SmartClinic.Mobile.Constants;

public static class ApiConstants
{
    // Base address used by the mobile app when communicating with the API.
    // This will be replaced with the hosted API address before deployment.
    public const string BaseUrl = "https://localhost:7001";

    // Authentication endpoints.
    public const string LoginEndpoint = "/api/auth/login";
    public const string RegisterEndpoint = "/api/auth/register";

    // Patient profile endpoints.
    public const string CurrentPatientEndpoint = "/api/patients/me";
    public const string UpdatePatientEndpoint = "/api/patients/me";

    // Appointment endpoints for the authenticated patient.
    public const string AppointmentsEndpoint = "/api/appointments";
    public const string UpcomingAppointmentEndpoint = "/api/appointments/upcoming";

    // Doctor endpoints used during appointment booking.
    public const string DoctorsEndpoint = "/api/doctors";
    public const string DoctorAvailabilityEndpoint = "/api/doctors/{0}/availability";

    // Patient check-in and queue endpoints.
    public const string CheckInAppointmentEndpoint = "/api/appointments/{0}/check-in";
    public const string QueueStatusEndpoint = "/api/appointments/{0}/queue";

    // Clinical record endpoints for the authenticated patient.
    public const string ConsultationsEndpoint = "/api/consultations";
    public const string ConsultationDetailsEndpoint = "/api/consultations/{0}";
    public const string PrescriptionsEndpoint = "/api/prescriptions";
    public const string ConsultationPrescriptionsEndpoint = "/api/consultations/{0}/prescriptions";

    // Notification endpoints for the authenticated patient.
    public const string NotificationsEndpoint = "/api/notifications";
    public const string UnreadNotificationsEndpoint = "/api/notifications/unread";
    public const string MarkNotificationReadEndpoint = "/api/notifications/{0}/read";
    public const string MarkAllNotificationsReadEndpoint = "/api/notifications/read-all";
}