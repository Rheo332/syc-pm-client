using Microsoft.UI.Xaml.Controls;
using syc_pm_client.Viewmodels;

namespace syc_pm_client.Views
{
    public sealed partial class AddUserPage : Page
    {
        public AddUserPage(AddUserViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}