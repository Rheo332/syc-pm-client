using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using syc_pm_client.Viewmodels;

namespace syc_pm_client.Views
{
    public sealed partial class RequestsPage : Page
    {
        public RequestsPage(RequestsViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            Loaded += RequestsPage_Loaded;
        }

        private async void RequestsPage_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= RequestsPage_Loaded;

            await ((RequestsViewModel)DataContext).LoadRequestsAsync();
        }
    }
}