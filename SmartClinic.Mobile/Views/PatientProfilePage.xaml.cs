using SmartClinic.Mobile.ViewModels;

namespace SmartClinic.Mobile.Views;

public partial class PatientProfilePage : ContentPage
{
    private readonly PatientProfileViewModel _viewModel;

    public PatientProfilePage(
        PatientProfileViewModel viewModel)
    {
        InitializeComponent();

        // Connect the profile page to its ViewModel.
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Refresh the patient's profile whenever the page is opened.
        await _viewModel.LoadProfileAsync();
    }
}