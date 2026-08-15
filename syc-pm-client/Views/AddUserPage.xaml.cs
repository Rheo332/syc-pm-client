using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using syc_pm_client.Viewmodels;
using Windows.System;

namespace syc_pm_client.Views
{
    public sealed partial class AddUserPage : Page
    {
        public AddUserPage(AddUserViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            PasswordBox.PasswordChanged += (s, e) =>
            {
                vm.Password = PasswordBox.Password;
            };

            RepeatPasswordBox.PasswordChanged += (s, e) =>
            {
                vm.RepeatPassword = RepeatPasswordBox.Password;
            };
        }

        private void Register_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                var vm = DataContext as AddUserViewModel;
                vm?.SubmitCommand.Execute(null);
            }
        }
    }
}