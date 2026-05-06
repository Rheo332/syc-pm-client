using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using syc_pm_client.Services.NewFolder;
using syc_pm_client.Views;

namespace syc_pm_client.Viewmodels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly INavigationService _nav;

        [ObservableProperty]
        private string username;

        [ObservableProperty]
        private string password;

        public LoginViewModel(INavigationService nav)
        {
            _nav = nav;
        }

        [RelayCommand]
        private void Login()
        {
            // TODO: Implement login logic here

            _nav.Navigate<MainPage>();
        }
    }


}
