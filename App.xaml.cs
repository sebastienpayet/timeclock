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

        private void App_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            MainViewModel MainViewModel = ServiceLocator.Current.GetInstance<MainViewModel>();
            MainViewModel.StopSessionIfNeeded();
        }
    }
}
