using SmartClinic.Mobile.ViewModels;

namespace SmartClinic.Mobile.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();

        // Connect the page to its ViewModel for data binding and commands.
        BindingContext = viewModel;
    }
}