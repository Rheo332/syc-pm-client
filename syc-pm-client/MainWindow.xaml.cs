using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using syc_pm_client.Services.Interfaces;
using syc_pm_client.Views;
using WinRT.Interop;

namespace syc_pm_client
{
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
            _nav.OnNavigate += Navigation_OnNavigate;

            ExtendsContentIntoTitleBar = true;
            SetTitleBar(TitleBar);

            var hWnd = WindowNative.GetWindowHandle(this);
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.Resize(new Windows.Graphics.SizeInt32(1250, 800));

            if (appWindow.Presenter is OverlappedPresenter presenter)
            {
                presenter.IsResizable = false;
                presenter.IsMaximizable = false;
            }

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
            DispatcherQueue.TryEnqueue(UpdateLogoutButtonVisibility);
        }

        private void Navigation_OnNavigate()
        {
            DispatcherQueue.TryEnqueue(UpdateLogoutButtonVisibility);
        }

        private void UpdateLogoutButtonVisibility()
        {
            if (_userSession.CurrentUser == null)
            {
                LogoutButton.Visibility = Visibility.Collapsed;
                BackButton.Visibility = Visibility.Collapsed;
            }
            else if (MainFrame.Content is MainPage)
            {
                LogoutButton.Visibility = Visibility.Visible;
                BackButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                LogoutButton.Visibility = Visibility.Collapsed;
                BackButton.Visibility = Visibility.Visible;
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            _userSession.Logout();
            _nav.Navigate<LoginPage>();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            _nav.Navigate<MainPage>();
        }
    }
}
