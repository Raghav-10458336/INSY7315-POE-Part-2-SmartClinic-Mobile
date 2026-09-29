using SmartClinic.Mobile.Models;
using SmartClinic.Mobile.ViewModels;

namespace SmartClinic.Mobile.Views;

public partial class RescheduleAppointmentPage : ContentPage
{
    private readonly RescheduleAppointmentViewModel _viewModel;

    public RescheduleAppointmentPage(
        RescheduleAppointmentViewModel viewModel)
    {
        InitializeComponent();

        // Connect the reschedule page to its ViewModel.
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    public async Task InitialiseAsync(Appointment appointment)
    {
        // Load the selected appointment and its available replacement times.
        await _viewModel.InitialiseAsync(appointment);
    }
}