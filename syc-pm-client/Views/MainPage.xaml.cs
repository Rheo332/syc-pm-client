using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using syc_pm_client.Viewmodels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace syc_pm_client.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage(MainViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            Loaded += MainPage_Loaded;
        }

        private void AccountsList_ItemClick(object? sender, ItemClickEventArgs e)
        {
            var vm = (MainViewModel)DataContext;
            if (vm.SelectedAccount == e.ClickedItem)
            {
                // unselect if item is already selected
                DispatcherQueue.TryEnqueue(() =>
                {
                    vm.SelectedAccount = null;
                });
            }
        }

        private async void MainPage_Loaded(object? sender, RoutedEventArgs e)
        {
            Loaded -= MainPage_Loaded;

            await ((MainViewModel)DataContext).LoadDataAsync();
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            await ((MainViewModel)DataContext).LoadDataAsync();
        }
    }
}