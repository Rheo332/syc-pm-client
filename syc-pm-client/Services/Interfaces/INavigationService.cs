using Microsoft.UI.Xaml.Controls;

namespace syc_pm_client.Services.Interfaces
{
    public interface INavigationService
    {
        public void Initialize(Frame frame);
        public void Navigate<T>(System.Action<T>? configure = null) where T : Page;
    }
}
