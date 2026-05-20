using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using syc_pm_client.Services.Interfaces;
using syc_pm_client.Views;
using System.Threading.Tasks;

namespace syc_pm_client.Viewmodels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly INavigationService _nav;
        public MainViewModel(INavigationService nav)
        {
            _nav = nav;
        }

        [RelayCommand]
        private async Task Logout()
        {
            _nav.Navigate<LoginPage>();
        }
    }
}
