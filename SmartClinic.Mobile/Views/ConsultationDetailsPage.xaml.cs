using SmartClinic.Mobile.Models;
using SmartClinic.Mobile.ViewModels;

namespace SmartClinic.Mobile.Views;

public partial class ConsultationDetailsPage : ContentPage
{
    private readonly ConsultationDetailsViewModel _viewModel;

    public ConsultationDetailsPage(
        ConsultationDetailsViewModel viewModel)
    {
        InitializeComponent();

        // Connect the consultation details page to its ViewModel.
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    public async Task InitialiseAsync(
        Consultation consultation)
    {
        // Load the selected consultation and its related prescriptions.
        await _viewModel.InitialiseAsync(consultation);
    }
}