using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using syc_pm_client.Viewmodels;
using System.ComponentModel;

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
            vm.PropertyChanged += Vm_PropertyChanged;

            Loaded += MainPage_Loaded;
        }

        private void Vm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainViewModel.SelectedAccount))
            {
                var vm = (MainViewModel)DataContext;
                if (vm.SelectedAccount != null)
                {
                    Column0.Width = new GridLength(2, GridUnitType.Star);
                    Column1.Width = new GridLength(1, GridUnitType.Star);
                }
                else
                {
                    Column0.Width = new GridLength(1, GridUnitType.Star);
                    Column1.Width = new GridLength(0);
                }
            }
        }

        private void AccountsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            var vm = (MainViewModel)DataContext;
            if (vm.SelectedAccount == e.ClickedItem)
            {
                // Unselect if the clicked item is already selected
                DispatcherQueue.TryEnqueue(() =>
                {
                    vm.SelectedAccount = null;
                });
            }
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainPage_Loaded;

            await ((MainViewModel)DataContext).LoadDataAsync();
        }
    }
}