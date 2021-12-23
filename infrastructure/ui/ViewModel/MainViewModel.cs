using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TimeClock.business.model.workSession;
using TimeClock.business.useCase.getSessionsTimeForADay;
using TimeClock.business.useCase.startAWorkSession;
using TimeClock.business.useCase.stopAWorkSession;

namespace TimeClock.infrastructure.ui.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public ICommand LoadEmployeesCommand { get; private set; }
        public ICommand SwitchTimerCommand { get; private set; }

        public String SwitchButtonImageUri { get; private set; }
        public bool isTimerRunning { get; private set; }

        // CDI
        public StartAWorkSession _startAWorkSession { get; private set; }
        public StopAWorkSession _stopAWorkSession { get; private set; }
        public GetSessionsTimeForADay _getSessionsTimeForADay { get; private set; }

        private const string PAUSE_IMAGE = "pause-button.png";
        private const string PLAY_IMAGE = "play-button.png";

        DateTime sessionStartTime;
        string currentSessionTimer;
        string daySessionsTimer;



        DispatcherTimer timer;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel(
                StartAWorkSession startAWorkSession,
                StopAWorkSession stopAWorkSession,
                GetSessionsTimeForADay getSessionsTimeForADay
            )
        {
            _startAWorkSession = startAWorkSession;
            _stopAWorkSession = stopAWorkSession;
            _getSessionsTimeForADay = getSessionsTimeForADay;


            // command relays
            LoadEmployeesCommand = new RelayCommand(LoadEmployeesMethod);
            SwitchTimerCommand = new RelayCommand(SwitchTimerMethod);

            // start values
            isTimerRunning = false;
            SwitchButtonImageUri = PLAY_IMAGE;

            timer = buildTimer();

            Application.Current.Dispatcher.Invoke(
            DispatcherPriority.ApplicationIdle,
            new Action(() =>
            {
                DaySessionsTimer = BuildTimerString(_getSessionsTimeForADay.Handle(new GetSessionsTimeForADayCommand(DateTime.Now)));
            }));
        }

        private DispatcherTimer buildTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += (s, e) =>
            {
                CurrentSessionTimer = BuildTimerString(DateTime.Now - sessionStartTime);
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

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("CurrentSessionTimer"));
                    }
                }
            }
            get
            {
                return currentSessionTimer;
            }
        }

        public string DaySessionsTimer
        {
            set
            {
                if (daySessionsTimer != value)
                {
                    daySessionsTimer = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("DaySessionsTimer"));
                    }
                }
            }
            get
            {
                return daySessionsTimer;
            }
        }

        private void LoadEmployeesMethod()
        {
            Messenger.Default.Send(
                new NotificationMessage(
                    _startAWorkSession.Handle(new StartAWorkSessionCommand()).Date.ToString()
                    )
                );
        }

        private void SwitchTimerMethod()
        {
            
            isTimerRunning = !isTimerRunning;
            if (isTimerRunning)
            {
                WorkSession session = _startAWorkSession.Handle(new StartAWorkSessionCommand());
                sessionStartTime = session.Date;
                timer.Start();
                SwitchButtonImageUri = PAUSE_IMAGE;
            }
            else
            {
                _stopAWorkSession.Handle(new StopAWorkSessionCommand());
                timer.Stop();
                SwitchButtonImageUri = PLAY_IMAGE;
                DaySessionsTimer = BuildTimerString(_getSessionsTimeForADay.Handle(new GetSessionsTimeForADayCommand(DateTime.Now)));
            }
            PropertyChanged(this, new PropertyChangedEventArgs("SwitchButtonImageUri"));
        }
    }
}