using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace TimeClock.ViewModel
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

        DateTime dateTime;
        DateTime startTime;
        String timeCounter;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            LoadEmployeesCommand = new RelayCommand(LoadEmployeesMethod);
            this.DateTime = DateTime.Now;
            this.startTime = DateTime.Now;
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);

            timer.Tick += (s, e) => { 
                this.DateTime = DateTime.Now;
                this.TimeCounter = BuildTimeCounter();
            };
            //définition de l'interval en ms (333 trois fois par minutes c'est suffisant pour que l'utilisateur ne remarque pas la différence)
            //avec l'heure system, mais à modifier selon ta précision et la performance...
            timer.Interval = TimeSpan.FromSeconds(1);
            //lancement de l'affichage de l'heure.
            timer.Start();
        }

        private string BuildTimeCounter()
        {
            TimeSpan interval = DateTime.Now - this.startTime;
            return string.Format("{0:D2}:{1:D2}:{2:D2}", interval.Hours, interval.Minutes, interval.Seconds);
        }

        public string TimeCounter
        {
            set
            {
                if (timeCounter != value)
                {
                    timeCounter = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("TimeCounter"));
                    }
                }
            }
            get
            {
                return timeCounter;
            }
        }

        public DateTime DateTime
        {
            set
            {
                if (dateTime != value)
                {
                    dateTime = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("DateTime"));
                    }
                }
            }
            get
            {
                return dateTime;
            }
        }

        private void LoadEmployeesMethod()
        {
            Messenger.Default.Send<NotificationMessage>(new NotificationMessage("Employees Loaded."));
        }



    }
}