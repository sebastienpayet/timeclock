using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TimeClock.business.model.workSession;
using TimeClock.business.useCase.exportData;
using TimeClock.business.useCase.getSessionsTimeForADay;
using TimeClock.business.useCase.startAWorkSession;
using TimeClock.business.useCase.stopAWorkSession;
using TimeClock.infrastructure.util;

namespace TimeClock.infrastructure.ui.ViewModel
{

    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        // command relays bindings
        public ICommand ExportDataCommand => new RelayCommand(ExportDataMethod);
        public ICommand SwitchTimerCommand => new RelayCommand(SwitchTimerMethod);
        public ICommand WindowClosingCommand => new RelayCommand<EventArgs>(ApplicationClosingMethod);

        // view bindings
        public new event PropertyChangedEventHandler PropertyChanged;
        public string SwitchButtonImageUri { get; private set; }
        
        // CDI
        public StartAWorkSession StartAWorkSession { get; private set; }
        public StopAWorkSession StopAWorkSession { get; private set; }
        public GetSessionsTimeForADay GetSessionsTimeForADay { get; private set; }
        public ExportData ExportData { get; private set; }

        // internal constants
        private const string PAUSE_IMAGE = "pack://application:,,,/Resources/pause-button.png";
        private const string PLAY_IMAGE = "pack://application:,,,/Resources/play-button.png";
        private const int MAX_IDLE_TIME_IN_SECONDS = 600;
        private readonly SoundPlayer SoundPlayer = new SoundPlayer(Properties.Resources.button_15);

        // internal variables
        private DateTime sessionStartTime;
        private string currentSessionTimerValue;
        private string daySessionsTimerValue;
        private readonly DispatcherTimer viewRefreshTimer;
        private bool IsSessionInProgress;

        public MainViewModel(
                StartAWorkSession startAWorkSession,
                StopAWorkSession stopAWorkSession,
                GetSessionsTimeForADay getSessionsTimeForADay,
                ExportData exportData
            )
        {
            Logger.Info("Init mainWindow");
            // CDI init
            StartAWorkSession = startAWorkSession;
            StopAWorkSession = stopAWorkSession;
            GetSessionsTimeForADay = getSessionsTimeForADay;
            ExportData = exportData;

            // init start vars values
            IsSessionInProgress = false;
            SwitchButtonImageUri = PLAY_IMAGE;

            // init timer
            viewRefreshTimer = BuildRefreshTimer();

            // init watch on power change
            SystemEvents.PowerModeChanged += OnPowerChange;

            // init current day timer value
            _ = Application.Current.Dispatcher.Invoke(
            DispatcherPriority.ApplicationIdle,
            new Action(() =>
            {
                RefreshDaySessionsDisplay();
            }));
        }



        private void OnPowerChange(object s, PowerModeChangedEventArgs e)
        {
            Logger.Info($"PowerChange event triggered with {e.Mode}");
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    RefreshDaySessionsDisplay();
                    break;
                case PowerModes.Suspend:
                    StopSession();
                    break;
                case PowerModes.StatusChange:
                default:
                    // do nothing
                    break;
            }
        }

        private DispatcherTimer BuildRefreshTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += (s, e) =>
            {
                CurrentSessionTimer = FormatUtils.BuildTimerString(DateTime.Now - sessionStartTime);

                if (SystemUtils.GetIdleTime() >= MAX_IDLE_TIME_IN_SECONDS)
                {
                    StopSession();
                    Messenger.Default.Send<NotificationMessage>(new NotificationMessage("Votre session de travail à été arrêtée pour cause d'inactivité."));
                }
            };

            timer.Interval = TimeSpan.FromMilliseconds(333);
            return timer;
        }

        private void RefreshDaySessionsDisplay()
        {
            DaySessionsTimer = FormatUtils.BuildTimerString(GetSessionsTimeForADay.Handle(new GetSessionsTimeForADayCommand(DateTime.Now)));
            Logger.Info("Day Time refreshed");
        }

        public string CurrentSessionTimer 
        {
            set
            {
                if (currentSessionTimerValue != value)
                {
                    currentSessionTimerValue = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentSessionTimer"));
                }
            }
            get => currentSessionTimerValue;
        }

        public string DaySessionsTimer
        {
            set
            {
                if (daySessionsTimerValue != value)
                {
                    daySessionsTimerValue = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DaySessionsTimer"));
                }
            }
            get => daySessionsTimerValue;
        }

        private void ExportDataMethod()
        {
            Logger.Info("Export asked from view");
            StopSession();
            _ = ExportData.Handle(new ExportDataCommand(DateTime.Now));
        }

        private void SwitchTimerMethod()
        {
            Logger.Info("Timer switch asked from view");
            if (!IsSessionInProgress)
            {
                StartSession();
            }
            else
            {
                StopSession();
            }
        }

        private void StartSession()
        {
            Logger.Info("Ask for a session start");
            if (!IsSessionInProgress)
            {
                WorkSession session = StartAWorkSession.Handle(new StartAWorkSessionCommand());
                sessionStartTime = session.Date;
                viewRefreshTimer.Start();
                SwitchButtonImageUri = PAUSE_IMAGE;
                SoundPlayer.Play();
                PropertyChanged(this, new PropertyChangedEventArgs("SwitchButtonImageUri"));
                IsSessionInProgress = true;
                Logger.Info("UI session start process terminated");
            }
        }

        public void StopSession()
        {
            Logger.Info("Ask for a session stop");
            if (IsSessionInProgress)
            {
                _ = StopAWorkSession.Handle(new StopAWorkSessionCommand());
                viewRefreshTimer.Stop();
                SwitchButtonImageUri = PLAY_IMAGE;
                RefreshDaySessionsDisplay();
                PropertyChanged(this, new PropertyChangedEventArgs("SwitchButtonImageUri"));
                SystemSounds.Beep.Play();
                IsSessionInProgress = false;
                Logger.Info("UI session stop process terminated");
            }
        }

        private void ApplicationClosingMethod(EventArgs obj)
        {
            Logger.Info("Application closing");
            if (IsSessionInProgress)
            {
                Logger.Info("Session is in progress, auto stop triggered");
                _ = StopAWorkSession.Handle(new StopAWorkSessionCommand());
                Messenger.Default.Send<NotificationMessage>(new NotificationMessage("Votre session de travail en cours vient d'être terminée."));
            }
        }
    }
}