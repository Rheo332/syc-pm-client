using Microsoft.UI.Xaml.Controls;
using System;

namespace syc_pm_client.Services.Interfaces
{
    public interface INavigationService
    {
        event Action OnNavigate;
        public void Initialize(Frame frame);
        public void Navigate<T>(Action<T>? configure = null) where T : Page;
    }
}
