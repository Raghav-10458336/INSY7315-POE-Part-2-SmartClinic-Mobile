using Microsoft.Extensions.DependencyInjection;
using SmartClinic.Mobile.Views;

namespace SmartClinic.Mobile
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            // Store the service provider so startup pages can be resolved
            // through the application's dependency injection container.
            _serviceProvider = serviceProvider;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var loginPage = _serviceProvider.GetRequiredService<LoginPage>();

            return new Window(loginPage);
        }
    }
}