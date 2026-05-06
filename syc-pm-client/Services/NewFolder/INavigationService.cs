using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syc_pm_client.Services.NewFolder
{
    public interface INavigationService
    {
        public void Initialize(Frame frame);
        public void Navigate<T>() where T : Page;
    }
}
