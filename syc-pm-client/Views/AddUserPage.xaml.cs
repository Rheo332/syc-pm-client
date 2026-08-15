using Microsoft.UI.Xaml;
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

            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(vm.Password))
                {
                    if (PasswordBox.Password != vm.Password)
                    {
                        PasswordBox.Password = vm.Password ?? string.Empty;
                    }
                }
                else if (e.PropertyName == nameof(vm.RepeatPassword))
                {
                    if (RepeatPasswordBox.Password != vm.RepeatPassword)
                    {
                        RepeatPasswordBox.Password = vm.RepeatPassword ?? string.Empty;
                    }
                }
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

        private void RevealModeCheckbox_Changed(object sender, RoutedEventArgs e)
        {
            if (RevealModeCheckBox.IsChecked == true)
            {
                PasswordBox.PasswordRevealMode = PasswordRevealMode.Visible;
                RepeatPasswordBox.PasswordRevealMode = PasswordRevealMode.Visible;
            }
            else
            {
                PasswordBox.PasswordRevealMode = PasswordRevealMode.Hidden;
                RepeatPasswordBox.PasswordRevealMode = PasswordRevealMode.Hidden;
            }
        }
    }
}