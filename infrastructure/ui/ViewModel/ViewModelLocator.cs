/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:TimeClock"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Threading;
using System.Windows;
using TimeClock.business.port.exporter;
using TimeClock.business.port.repository;
using TimeClock.business.useCase.exportData;
using TimeClock.business.useCase.getSessionsTimeForADay;
using TimeClock.business.useCase.startAWorkSession;
using TimeClock.business.useCase.stopAWorkSession;
using TimeClock.infrastructure.exporter.excelExporter;
using TimeClock.infrastructure.repository.sqlLite;
using TimeClock.infrastructure.repository.sqlLite.workSession;
using TimeClock.infrastructure.ui.ViewModel;
using TimeClock.infrastructure.util;

namespace TimeClock.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        readonly Mutex mutex;
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            mutex = new Mutex(true, "TimeClockInstance", out bool isnew);
            if (!isnew)
            {
                _ = MessageBox.Show("TimeClock est déjà en cours d'exécution.");
                Environment.Exit(0);
            }

            SystemUtils.ConfigureAutoStart();

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            // CDI configuration
            // implementations
            SimpleIoc.Default.Register<TimeClockContext>();
            SimpleIoc.Default.Register<IWorkSessionRepository, SqliteWorkSessionRepository>();
            SimpleIoc.Default.Register<IExporter, ExcelExporter>();
            // uses cases
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<StartAWorkSession>();
            SimpleIoc.Default.Register<StopAWorkSession>();
            SimpleIoc.Default.Register<GetSessionsTimeForADay>();
            SimpleIoc.Default.Register<ExportData>();
            // utils
            Messenger.Default.Register<NotificationMessage>(this, NotifyUserMethod);

        }

        private void App_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            // Ask the user if they want to allow the session to end
            string msg = string.Format("{0}. End session?", e.ReasonSessionEnding);
            MessageBoxResult result = MessageBox.Show(msg, "Session Ending", MessageBoxButton.YesNo);

            // End session, if specified
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        public MainViewModel MainViewModel => ServiceLocator.Current.GetInstance<MainViewModel>();


        private void NotifyUserMethod(NotificationMessage message)
        {
            _ = MessageBox.Show(message.Notification);
        }

        public static void Cleanup()
        {
            // todo
        }
    }
}