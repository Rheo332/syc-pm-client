using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using syc_pm_client.Services.Interfaces;
using syc_pm_client.Views;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace syc_pm_client
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private readonly INavigationService _nav;
        private readonly IUserSessionService _userSession;

        public MainWindow(INavigationService nav, IUserSessionService userSession)
        {
            InitializeComponent();
            _nav = nav;
            _userSession = userSession;

            _userSession.OnSessionChanged += UserSession_OnSessionChanged;

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(TitleBar);

            var hWnd = WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32(1200, 700));

            var displayArea = DisplayArea.GetFromWindowId(windowId, DisplayAreaFallback.Primary);
            var centerPosition = new Windows.Graphics.PointInt32(
                (displayArea.WorkArea.Width - appWindow.Size.Width) / 2,
                (displayArea.WorkArea.Height - appWindow.Size.Height) / 2
            );
            appWindow.Move(centerPosition);

            nav.Initialize(MainFrame);
            nav.Navigate<LoginPage>();
            UpdateLogoutButtonVisibility();
        }

        private void UserSession_OnSessionChanged()
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                UpdateLogoutButtonVisibility();
            });
        }

        private void UpdateLogoutButtonVisibility()
        {
            LogoutButton.Visibility = _userSession.CurrentUser != null ? Visibility.Visible : Visibility.Collapsed;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _userSession.Logout();
            _nav.Navigate<LoginPage>();
        }
    }
}
