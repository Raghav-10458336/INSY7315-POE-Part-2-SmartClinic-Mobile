using SmartClinic.Mobile.ViewModels;

namespace SmartClinic.Mobile.Views;

public partial class BookAppointmentPage : ContentPage
{
    private readonly BookAppointmentViewModel _viewModel;

    public BookAppointmentPage(BookAppointmentViewModel viewModel)
    {
        InitializeComponent();

        // Connect the booking page to its ViewModel.
        _viewModel = viewModel;
        BindingContext = _viewModel;

        // Return to the appointments list after a successful booking.
        _viewModel.AppointmentBooked += OnAppointmentBooked;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Load the available doctors when the booking page opens.
        await _viewModel.LoadDoctorsAsync();
    }

    private async void OnViewTimesClicked(object? sender, EventArgs e)
    {
        // Load appointment slots for the doctor selected by the patient.
        await _viewModel.LoadAvailabilityAsync();
    }

    private async void OnAppointmentBooked(object? sender, EventArgs e)
    {
        // Return to the appointments list after the booking is saved.
        if (Navigation.NavigationStack.Count > 1)
        {
            await Navigation.PopAsync();
        }
    }
}