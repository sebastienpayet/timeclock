using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
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
        // command relays
        public ICommand ExportDataCommand => new RelayCommand(ExportDataMethod);
        public ICommand SwitchTimerCommand => new RelayCommand(SwitchTimerMethod);
        public ICommand WindowClosingCommand => new RelayCommand<EventArgs>(ApplicationClosingMethod);

        public new event PropertyChangedEventHandler PropertyChanged;
        public string SwitchButtonImageUri { get; private set; }
        public bool IsSessionInProgress { get; private set; }

        // CDI
        public StartAWorkSession StartAWorkSession { get; private set; }
        public StopAWorkSession StopAWorkSession { get; private set; }
        public GetSessionsTimeForADay GetSessionsTimeForADay { get; private set; }
        public ExportData ExportData { get; private set; }

        private const string PAUSE_IMAGE = "pack://application:,,,/Resources/pause-button.png";
        private const string PLAY_IMAGE = "pack://application:,,,/Resources/play-button.png";
        private const int MAX_IDLE_TIME_IN_SECONDS = 600;
        private readonly SoundPlayer SoundPlayer = new SoundPlayer(Properties.Resources.button_15);

        private DateTime sessionStartTime;
        private string currentSessionTimer;
        private string daySessionsTimer;
        private readonly DispatcherTimer timer;

        public MainViewModel(
                StartAWorkSession startAWorkSession,
                StopAWorkSession stopAWorkSession,
                GetSessionsTimeForADay getSessionsTimeForADay,
                ExportData exportData
            )
        {
            // CDI init
            StartAWorkSession = startAWorkSession;
            StopAWorkSession = stopAWorkSession;
            GetSessionsTimeForADay = getSessionsTimeForADay;
            ExportData = exportData;

            // init start vars values
            IsSessionInProgress = false;
            SwitchButtonImageUri = PLAY_IMAGE;

            // init timer
            timer = BuildTimer();

            // init watch on power change
            SystemEvents.PowerModeChanged += OnPowerChange;

            // init current day timer value
            _ = Application.Current.Dispatcher.Invoke(
            DispatcherPriority.ApplicationIdle,
            new Action(() =>
            {
                RefreshDaySessionsTimer();
            }));
        }



        private void OnPowerChange(object s, PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    RefreshDaySessionsTimer();
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

        private DispatcherTimer BuildTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += (s, e) =>
            {
                CurrentSessionTimer = FormatUtils.BuildTimerString(DateTime.Now - sessionStartTime);

                if (SystemUtils.GetIdleTime() >= MAX_IDLE_TIME_IN_SECONDS)
                {
                    StopSession();
                    _ = MessageBox.Show("Votre session de travail à été arrêtée pour cause d'inactivité.", Properties.Resources.AppName);
                }
            };

            timer.Interval = TimeSpan.FromMilliseconds(333);
            return timer;
        }

        private void RefreshDaySessionsTimer()
        {
            DaySessionsTimer = FormatUtils.BuildTimerString(GetSessionsTimeForADay.Handle(new GetSessionsTimeForADayCommand(DateTime.Now)));
        }

        public string CurrentSessionTimer
        {
            set
            {
                if (currentSessionTimer != value)
                {
                    currentSessionTimer = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CurrentSessionTimer"));
                }
            }
            get => currentSessionTimer;
        }

        public string DaySessionsTimer
        {
            set
            {
                if (daySessionsTimer != value)
                {
                    daySessionsTimer = value;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("DaySessionsTimer"));
                }
            }
            get => daySessionsTimer;
        }

        private void ExportDataMethod()
        {
            StopSession();
            _ = ExportData.Handle(new ExportDataCommand(DateTime.Now));
        }

        private void SwitchTimerMethod()
        {
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
            if (!IsSessionInProgress)
            {
                WorkSession session = StartAWorkSession.Handle(new StartAWorkSessionCommand());
                sessionStartTime = session.Date;
                timer.Start();
                SwitchButtonImageUri = PAUSE_IMAGE;
                SoundPlayer.Play();
                PropertyChanged(this, new PropertyChangedEventArgs("SwitchButtonImageUri"));
                IsSessionInProgress = true;
            }
        }

        public void StopSession()
        {
            if (IsSessionInProgress)
            {
                _ = StopAWorkSession.Handle(new StopAWorkSessionCommand());
                timer.Stop();
                SwitchButtonImageUri = PLAY_IMAGE;
                RefreshDaySessionsTimer();
                PropertyChanged(this, new PropertyChangedEventArgs("SwitchButtonImageUri"));
                SystemSounds.Beep.Play();
                IsSessionInProgress = false;
            }
        }

        private void ApplicationClosingMethod(EventArgs obj)
        {
            if (IsSessionInProgress)
            {
                _ = StopAWorkSession.Handle(new StopAWorkSessionCommand());
                _ = MessageBox.Show("Votre session de travail en cours vient d'être terminée.", Properties.Resources.AppName);
            }
        }
    }
}