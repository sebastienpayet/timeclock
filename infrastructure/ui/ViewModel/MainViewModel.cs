using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.ComponentModel;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TimeClock.business.model.workSession;
using TimeClock.business.useCase.exportData;
using TimeClock.business.useCase.getSessionsTimeForADay;
using TimeClock.business.useCase.startAWorkSession;
using TimeClock.business.useCase.stopAWorkSession;

namespace TimeClock.infrastructure.ui.ViewModel
{

    internal struct LASTINPUTINFO
    {
        public uint cbSize;

        public uint dwTime;
    }

    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
        // command relays
        public ICommand ExportDataCommand { get; private set; }
        public ICommand SwitchTimerCommand { get; private set; }
        public ICommand WindowClosingCommand => new RelayCommand<EventArgs>(ApplicationClosingMethod);

        public new event PropertyChangedEventHandler PropertyChanged;
        public string SwitchButtonImageUri { get; private set; }
        public bool IsTimerRunning { get; private set; }

        // CDI
        public StartAWorkSession StartAWorkSession { get; private set; }
        public StopAWorkSession StopAWorkSession { get; private set; }
        public GetSessionsTimeForADay GetSessionsTimeForADay { get; private set; }
        public ExportData ExportData { get; private set; }

        private const string PAUSE_IMAGE = "pause-button.png";
        private const string PLAY_IMAGE = "play-button.png";
        private const int MAX_IDLE_TIME_IN_SECONDS = 10;

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

            // command relays
            ExportDataCommand = new RelayCommand(ExportDataMethod);
            SwitchTimerCommand = new RelayCommand(SwitchTimerMethod);

            // init start vars values
            IsTimerRunning = false;
            SwitchButtonImageUri = PLAY_IMAGE;

            // init timer
            timer = BuildTimer();

            // init current day timer value
            _ = Application.Current.Dispatcher.Invoke(
            DispatcherPriority.ApplicationIdle,
            new Action(() =>
            {
                DaySessionsTimer = BuildTimerString(GetSessionsTimeForADay.Handle(new GetSessionsTimeForADayCommand(DateTime.Now)));
            }));
        }

        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("Kernel32.dll")]
        private static extern uint GetLastError();

        public static uint GetIdleTime()
        {
            LASTINPUTINFO lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)Marshal.SizeOf(lastInPut);
            GetLastInputInfo(ref lastInPut);

            return (uint)((((Environment.TickCount & int.MaxValue) - (lastInPut.dwTime & int.MaxValue)) & int.MaxValue) / 1000);
        }

        private DispatcherTimer BuildTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += (s, e) =>
            {
                CurrentSessionTimer = BuildTimerString(DateTime.Now - sessionStartTime);

                // todo a nettoyer
                if (IsTimerRunning && GetIdleTime() >= MAX_IDLE_TIME_IN_SECONDS)
                {
                    StopSession();
                    IsTimerRunning = false;
                    MessageBox.Show("Votre session de travail à été arrêtée pour cause d'inactivité.", "TimeClock");
                }
            };
            timer.Interval = TimeSpan.FromMilliseconds(333);
            return timer;
        }

        private string BuildTimerString(TimeSpan interval)
        {
            return string.Format("{0:D2}:{1:D2}:{2:D2}", interval.Hours, interval.Minutes, interval.Seconds);
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
            _ = ExportData.Handle(new ExportDataCommand(DateTime.Now));
        }

        private void SwitchTimerMethod()
        {

            IsTimerRunning = !IsTimerRunning;
            if (IsTimerRunning)
            {
                WorkSession session = StartAWorkSession.Handle(new StartAWorkSessionCommand());
                sessionStartTime = session.Date;
                timer.Start();
                SwitchButtonImageUri = PAUSE_IMAGE;
                SystemSounds.Exclamation.Play();
                PropertyChanged(this, new PropertyChangedEventArgs("SwitchButtonImageUri"));
            }
            else
            {
                StopSession();
            }
            
        }

        private void StopSession()
        {
            _ = StopAWorkSession.Handle(new StopAWorkSessionCommand());
            timer.Stop();
            SwitchButtonImageUri = PLAY_IMAGE;
            DaySessionsTimer = BuildTimerString(GetSessionsTimeForADay.Handle(new GetSessionsTimeForADayCommand(DateTime.Now)));
            PropertyChanged(this, new PropertyChangedEventArgs("SwitchButtonImageUri"));
            SystemSounds.Exclamation.Play();
        }

        private void ApplicationClosingMethod(EventArgs obj)
        {
            if (IsTimerRunning)
            {
               StopAWorkSession.Handle(new StopAWorkSessionCommand());
                _ = MessageBox.Show("Votre session de travail en cours vient d'être terminée.", "TimeClock");
            }
        }
    }
}