using CommonServiceLocator;
using Microsoft.Win32;
using System.Windows;
using TimeClock.infrastructure.ui.ViewModel;

namespace TimeClock
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private MainViewModel _mainViewModel;

        public App()
        {
            // init watch on power change
            SystemEvents.PowerModeChanged += OnPowerChange;
        }

        private MainViewModel MainViewModel
        {
            get
            {
                if (_mainViewModel != null)
                {
                    return _mainViewModel;
                }
                _mainViewModel = ServiceLocator.Current.GetInstance<MainViewModel>();
                return _mainViewModel;
            }
        }


        private void App_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            Logger.Info("Windows session ending detected");
            MainViewModel.StopSession();
        }

        private void OnPowerChange(object s, PowerModeChangedEventArgs e)
        {
            Logger.Info($"PowerChange event triggered with {e.Mode}");
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    MainViewModel.RefreshDaySessionsDisplay();
                    break;
                case PowerModes.Suspend:
                    MainViewModel.StopSession();
                    break;
                case PowerModes.StatusChange:
                default:
                    // do nothing
                    break;
            }
        }
    }
}
