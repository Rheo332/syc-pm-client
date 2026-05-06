using Microsoft.UI.Xaml;
using syc_pm_client.Services.NewFolder;
using syc_pm_client.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace syc_pm_client
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow(INavigationService nav)
        {
            InitializeComponent();

            nav.Initialize(MainFrame);
            nav.Navigate<LoginPage>();
        }
    }
}
