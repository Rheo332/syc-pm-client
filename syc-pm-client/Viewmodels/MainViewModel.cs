using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using syc_pm_client.Services.Interfaces;
using syc_pm_client.Views;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace syc_pm_client.Viewmodels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly INavigationService _nav;
        public MainViewModel(INavigationService nav)
        {
            _nav = nav;

            LoadDummyData();
        }

        [RelayCommand]
        private async Task Logout()
        {
            _nav.Navigate<LoginPage>();
        }

        public ObservableCollection<Accounts> Accounts { get; } = new();

        private void LoadDummyData()
        {

            for (int i = 0; i < 10; i++)
                Accounts.Add(new Accounts
                {
                    Name = $"Account {i + 1}",
                    Username = $"user{i + 1}",
                    Email = $"user{i + 1}@example.com",
                    Password = "Password",
                    URL = $"https://www.example.com/user{i + 1}",
                    Notes = $"Notes for Account {i + 1}"
                });
        }
    }

    public class Accounts
    {
        public string Name { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string URL { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}