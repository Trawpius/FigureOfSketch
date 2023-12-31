using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Windows;
using System.Drawing;

namespace FigureOfSketch.Objects
{
    public class GrabBag: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        #region Private Properties
        private string _dir;
        private string _currentimg;
        private double _timeTotal;
        private double _timeRemaining;
        private TimeSpan _span;
        private int _blur;
        #endregion

        #region Initialization
        public GrabBag()
        {
            Rand = new Random();
            Blur = 0;
        }
        #endregion


        #region Private Properties
        private string[] Files { get; set; }
        private int Min { get; set; }
        private int Max { get; set; }
        private Random Rand { get; set; }
        private BagState State { get; set; } = BagState.Stopped;
        #endregion

        #region Public Properties

        public string ActiveDirectory
        {
            get { return _dir; }
            set
            {
                _dir = value;
                Files = (from x in Directory.GetFiles(value, "*.*", SearchOption.AllDirectories)
                         where x.ToLower().EndsWith("jpg") ||
                         x.ToLower().EndsWith("jpeg") ||
                         x.ToLower().EndsWith("bmp") ||
                         x.ToLower().EndsWith("png")
                         select x).ToArray();
            }
        }
        public int Blur
        {
            get { return _blur; }
            set
            {
                _blur = value;
                OnPropertyChanged();
            }
        }
        public string CurrentImg
        {
            get { return _currentimg; }
            set
            {
                _currentimg = value;
                OnPropertyChanged();
            }
        }
        public bool IsRunning { get { return (State!=BagState.Stopped); } }
        public TimeSpan Span
        {
            get { return _span; }
            set
            {
                _span = value;
                OnPropertyChanged();
            }
        }
        public double TimeRemaining
        {
            get { return _timeRemaining; }
            set
            {
                _timeRemaining = value;
                OnPropertyChanged();
            }
        }
        public double TimeTotal
        {
            get { return _timeTotal; }
            set
            {
                _timeTotal = value;
                OnPropertyChanged();
            }
        }
        #endregion


        public void Execute()
        {
            if (!Initialize())
                return;

            while (State != BagState.Stopped)
            {
                State = BagState.Running;
                int ind = Convert.ToInt32(Math.Round(Rand.NextDouble() * (Max - Min) + Min));
                Action act = new Action(() => 
                {
                    CurrentImg = Files[ind];
                });

                Application.Current.Dispatcher.BeginInvoke(act);

                Stopwatch s = new Stopwatch();
                s.Start();

                while (s.Elapsed < Span && (State != BagState.Stopped && State != BagState.Skipped))
                {
                    // Blur Image if Paused
                    if(State==BagState.Paused)
                    {
                        Blur = 200;
                        while (State == BagState.Paused) { }
                        Blur = 0;
                    }

                    TimeRemaining =  ((Span - s.Elapsed).TotalSeconds / TimeTotal)*100;
                }
                s.Stop();
                s.Reset();
            }
        }

        private bool Initialize()
        {
            Min = 0;
            Max = Files.Count() - 1;

            if (Max < 1)
                return false;

            State = BagState.Running;
            TimeTotal = Span.TotalSeconds;

            return true;
        }

        public void Cancel()
        {
            State = BagState.Stopped;
        }

        public void Next()
        {
            State = BagState.Skipped;
        }

        public void Pause()
        {
            State = BagState.Paused;
        }

        public void Unpause()
        {
            State = BagState.Running;
        }

        #region Event Handler
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }

    public enum BagState
    {
        Running,
        Paused,
        Skipped,
        Stopped
    }
}
