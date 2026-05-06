using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using syc_pm_client.Services.Interfaces;
using syc_pm_client.Views;
using System.Threading.Tasks;

namespace syc_pm_client.Viewmodels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly INavigationService _nav;

        [ObservableProperty]
        public partial string? Username { get; set; }

        [ObservableProperty]
        public partial string? Password { get; set; }

        public LoginViewModel(INavigationService nav)
        {
            _nav = nav;
        }

        [RelayCommand]
        private async Task Login()
        {
            if (Username != null && Username != "Kevin")
            {
                _nav.Navigate<MainPage>();
            }

        }
    }


}
