using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace TimeClock
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon notifyIcon;
        private ContextMenu contextMenu;

        public MainWindow()
        {
            InitializeComponent();

#if !DEBUG
            SystemUtil.ConfigureAutoStart();
#endif

            SetWindowLocation();
            SetNotificationIcon();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }

            base.OnStateChanged(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        protected override void OnClosed(EventArgs e)
        {
            notifyIcon.Icon.Dispose();
            notifyIcon.Dispose();
            base.OnClosed(e);
        }

        private void SetWindowLocation()
        {
            // first open windows location
            WindowStartupLocation = WindowStartupLocation.Manual;
            Left = SystemParameters.WorkArea.Width - Width;
            Top = SystemParameters.WorkArea.Height - Height;
        }

        private void SetNotificationIcon()
        {
            // context menu setup
            contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add("&Ouvrir");
            contextMenu.MenuItems.Add("&Quitter");
            contextMenu.MenuItems[0].Click += delegate (object sender, EventArgs args)
            {
                Show();
            };
            contextMenu.MenuItems[1].Click += delegate (object sender, EventArgs args)
            {
                Application.Current.Shutdown();
            };

            // notifyIcon setup
            Uri iconUri = new Uri("pack://application:,,,/Main.ico", UriKind.RelativeOrAbsolute);
            var iconStream = Application.GetResourceStream(iconUri)?.Stream;
            notifyIcon = new NotifyIcon
            {
                Icon = new Icon(iconStream),
                Visible = true,
                Text = "TimeClock",
                ContextMenu = contextMenu
            };

            notifyIcon.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    Show();
                    WindowState = WindowState.Normal;
                };
        }
    }
}
