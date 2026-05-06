using Microsoft.UI.Xaml.Controls;

namespace syc_pm_client.Services.NewFolder
{
    public interface INavigationService
    {
        public void Initialize(Frame frame);
        public void Navigate<T>() where T : Page;
    }
}
