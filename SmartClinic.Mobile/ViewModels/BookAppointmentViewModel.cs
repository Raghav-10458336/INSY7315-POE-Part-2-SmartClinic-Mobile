using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SmartClinic.Mobile.DTOs;
using SmartClinic.Mobile.Models;
using SmartClinic.Mobile.Services;

namespace SmartClinic.Mobile.ViewModels;

public partial class BookAppointmentViewModel : BaseViewModel
{
    private readonly IDoctorService _doctorService;
    private readonly IAppointmentService _appointmentService;

    public ObservableCollection<Doctor> Doctors { get; } = [];
    public ObservableCollection<DoctorAvailability> AvailableSlots { get; } = [];

    [ObservableProperty]
    private Doctor? selectedDoctor;

    [ObservableProperty]
    private DoctorAvailability? selectedSlot;

    [ObservableProperty]
    private string reasonForVisit = string.Empty;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool hasError;

    [ObservableProperty]
    private bool hasDoctors;

    [ObservableProperty]
    private bool hasAvailableSlots;

    [ObservableProperty]
    private bool hasSelectedDoctor;

    [ObservableProperty]
    private string successMessage = string.Empty;

    [ObservableProperty]
    private bool hasSuccess;

    public BookAppointmentViewModel(
        IDoctorService doctorService,
        IAppointmentService appointmentService)
    {
        _doctorService = doctorService;
        _appointmentService = appointmentService;

        Title = "Book Appointment";
    }

    partial void OnSelectedDoctorChanged(Doctor? value)
    {
        // Update the UI when the patient selects or clears a doctor.
        HasSelectedDoctor = value is not null;

        // Clear availability from a previously selected doctor.
        AvailableSlots.Clear();
        SelectedSlot = null;
        HasAvailableSlots = false;

        ClearError();
        ClearSuccess();
    }
    public async Task LoadDoctorsAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();
            ClearSuccess();

            Doctors.Clear();

            // Load doctors available for patient appointment booking.
            var doctors = await _doctorService.GetDoctorsAsync();

            foreach (var doctor in doctors
                         .OrderBy(d => d.LastName)
                         .ThenBy(d => d.FirstName))
            {
                Doctors.Add(doctor);
            }

            HasDoctors = Doctors.Count > 0;
        }
        catch (Exception)
        {
            ShowError("Unable to load the available doctors.");
            HasDoctors = false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task LoadAvailabilityAsync()
    {
        AvailableSlots.Clear();
        SelectedSlot = null;
        HasAvailableSlots = false;

        if (SelectedDoctor is null)
        {
            ShowError("Please select a doctor first.");
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();
            ClearSuccess();

            // Load appointment slots belonging to the selected doctor.
            var availability = await _doctorService
                .GetDoctorAvailabilityAsync(SelectedDoctor.Id);

            foreach (var slot in availability
                         .Where(a => a.IsAvailable)
                         .OrderBy(a => a.StartDateTime))
            {
                AvailableSlots.Add(slot);
            }

            HasAvailableSlots = AvailableSlots.Count > 0;

            if (!HasAvailableSlots)
            {
                ShowError("No available appointment times were found for this doctor.");
            }
        }
        catch (Exception)
        {
            ShowError("Unable to load the doctor's availability.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CreateAppointmentAsync()
    {
        if (IsBusy)
        {
            return;
        }

        ClearError();
        ClearSuccess();

        // Validate the patient's booking selections before submission.
        if (SelectedDoctor is null)
        {
            ShowError("Please select a doctor.");
            return;
        }

        if (SelectedSlot is null)
        {
            ShowError("Please select an available appointment time.");
            return;
        }

        if (string.IsNullOrWhiteSpace(ReasonForVisit))
        {
            ShowError("Please enter a reason for your visit.");
            return;
        }

        try
        {
            IsBusy = true;

            var request = new CreateAppointmentRequest
            {
                DoctorId = SelectedDoctor.Id,
                AppointmentDateTime = SelectedSlot.StartDateTime,
                ReasonForVisit = ReasonForVisit.Trim()
            };

            // Submit the appointment through the authenticated API service.
            var appointment =
                await _appointmentService.CreateAppointmentAsync(request);

            if (appointment is null)
            {
                ShowError("Unable to book the appointment. Please try again.");
                return;
            }

            SuccessMessage = "Your appointment has been booked successfully.";
            HasSuccess = true;
        }
        catch (Exception)
        {
            ShowError("Unable to book the appointment. Please try again.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void ShowError(string message)
    {
        ErrorMessage = message;
        HasError = true;
    }

    private void ClearError()
    {
        ErrorMessage = string.Empty;
        HasError = false;
    }

    private void ClearSuccess()
    {
        SuccessMessage = string.Empty;
        HasSuccess = false;
    }
}