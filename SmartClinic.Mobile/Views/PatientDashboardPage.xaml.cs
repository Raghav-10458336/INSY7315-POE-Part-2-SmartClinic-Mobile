using SmartClinic.Mobile.ViewModels;

namespace SmartClinic.Mobile.Views;

public partial class PatientDashboardPage : ContentPage
{
    private readonly PatientDashboardViewModel _viewModel;

    public PatientDashboardPage(PatientDashboardViewModel viewModel)
    {
        InitializeComponent();

        // Connect the dashboard to its ViewModel.
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Refresh the patient's information whenever the dashboard appears.
        await _viewModel.LoadPatientAsync();
    }
}