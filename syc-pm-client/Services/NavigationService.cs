using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml.Controls;
using syc_pm_client.Services.Interfaces;
using System;

namespace syc_pm_client.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _provider;
        private Frame _frame;

        public NavigationService(IServiceProvider provider)
        {
            _provider = provider;
        }

        public void Initialize(Frame frame)
        {
            _frame = frame;
        }

        public void Navigate<T>() where T : Page
        {
            var page = _provider.GetRequiredService<T>();
            _frame.Content = page;
        }
    }
}
