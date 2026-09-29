using SmartClinic.Mobile.ViewModels;

namespace SmartClinic.Mobile.Views;

public partial class PrescriptionsPage : ContentPage
{
    private readonly PrescriptionsViewModel _viewModel;

    public PrescriptionsPage(
        PrescriptionsViewModel viewModel)
    {
        InitializeComponent();

        // Connect the prescriptions page to its ViewModel.
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Refresh prescriptions whenever the patient opens this page.
        await _viewModel.LoadPrescriptionsAsync();
    }
}