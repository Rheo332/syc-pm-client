using Microsoft.UI.Xaml.Controls;
using syc_pm_client.Viewmodels;

namespace syc_pm_client.Views
{
    public sealed partial class MakeRequestPage : Page
    {
        public MakeRequestPage(MakeRequestViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}