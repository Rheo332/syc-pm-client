using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using syc_pm_client.Services;
using syc_pm_client.Services.Interfaces;
using syc_pm_client.Viewmodels;
using syc_pm_client.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace syc_pm_client
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static IHost? Host { get; private set; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Shared HttpClient
                    services.AddSingleton(sp => new System.Net.Http.HttpClient
                    {
                        //BaseAddress = new System.Uri("https://csy-projektarbeit.studlp2.hshl.de/")
                        BaseAddress = new System.Uri("https://localhost:7166/")
                    });

                    // Services
                    services.AddSingleton<IUserSessionService, UserSessionService>();
                    services.AddSingleton<INavigationService, NavigationService>();
                    services.AddSingleton<IAuthenticationService, AuthenticationService>();
                    services.AddSingleton<IPwEntryService, PwEntryService>();

                    // Views
                    services.AddTransient<LoginPage>();
                    services.AddTransient<MainPage>();
                    services.AddTransient<AddEntryPage>();

                    // ViewModels
                    services.AddTransient<LoginViewModel>();
                    services.AddTransient<MainViewModel>();
                    services.AddTransient<AddEntryViewModel>();

                    // Window
                    services.AddSingleton<MainWindow>();
                })
                .Build();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var window = Host?.Services.GetRequiredService<MainWindow>();
            window?.Activate();
        }
    }
}
