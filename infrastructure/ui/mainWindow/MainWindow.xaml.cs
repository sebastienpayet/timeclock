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
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();
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
            Logger.Debug("Window Closing intecepted");
            e.Cancel = true;
            Hide();
        }

        protected override void OnClosed(EventArgs e)
        {
            Logger.Info("Killing notification icon");
            notifyIcon.Icon.Dispose();
            notifyIcon.Dispose();
            base.OnClosed(e);
        }


        private void SetWindowLocation()
        {
            Logger.Info("Setting up window location");
            // first open windows location
            WindowStartupLocation = WindowStartupLocation.Manual;
            Left = SystemParameters.WorkArea.Width - Width;
            Top = SystemParameters.WorkArea.Height - Height;
        }

        private void SetNotificationIcon()
        {
            Logger.Info("Setting up notification Icon");
            // context menu setup
            contextMenu = new ContextMenu();
            _ = contextMenu.MenuItems.Add("&Ouvrir");
            _ = contextMenu.MenuItems.Add("&Quitter");
            contextMenu.MenuItems[0].Click += delegate (object sender, EventArgs args)
            {
                Show();
            };
            contextMenu.MenuItems[1].Click += delegate (object sender, EventArgs args)
            {
                Application.Current.Shutdown();
            };

            // notifyIcon setup
            Icon icon = (Icon)Properties.Resources.MainIcon;
            notifyIcon = new NotifyIcon
            {
                Icon = icon,
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
