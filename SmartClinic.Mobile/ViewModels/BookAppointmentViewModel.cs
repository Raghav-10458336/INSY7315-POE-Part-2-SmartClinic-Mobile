using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using SmartClinic.Mobile.Models;
using SmartClinic.Mobile.Services;

namespace SmartClinic.Mobile.ViewModels;

public partial class BookAppointmentViewModel : BaseViewModel
{
    private readonly IDoctorService _doctorService;

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

    public BookAppointmentViewModel(IDoctorService doctorService)
    {
        _doctorService = doctorService;
        Title = "Book Appointment";
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
            return;
        }

        try
        {
            IsBusy = true;
            ClearError();

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
}