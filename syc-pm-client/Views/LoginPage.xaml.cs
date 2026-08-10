using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using syc_pm_client.Viewmodels;
using Windows.System;

namespace syc_pm_client.Views
{
    public sealed partial class LoginPage : Page
    {
        public LoginPage(LoginViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            PasswordBox.PasswordChanged += (s, e) =>
            {
                vm.Password = PasswordBox.Password;
            };
        }

        private void Login_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                var vm = DataContext as LoginViewModel;
                vm?.LoginCommand.Execute(null);
            }
        }
    }
}
