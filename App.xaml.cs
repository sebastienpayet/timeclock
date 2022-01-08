using CommonServiceLocator;
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

        private void App_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            Logger.Info("Windows session ending detected");
            
            MainViewModel MainViewModel = ServiceLocator.Current.GetInstance<MainViewModel>();
            MainViewModel.StopSession();
        }
    }
}
