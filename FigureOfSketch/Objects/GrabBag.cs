using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private int _imgHeight;
        private int _imgWidth;
        private double _timeRemaining;
        private TimeSpan _span;
        #endregion

        #region Initialization
        public GrabBag()
        {
            Rand = new Random();
        }
        #endregion

        #region Private Properties
        private string[] Files { get; set; }

        private Random Rand { get; set; }

        private bool Skipped { get; set; } = false;

        private bool Cancelled { get; set; } = false;

        private int Min { get; set; }
        private int Max { get; set; }
        #endregion

        #region Public Properties
        public string CurrentImg
        {
            get { return _currentimg; }
            set
            {
                _currentimg = value;
                OnPropertyChanged();
            }
        }

        public int ImgHeight
        {
            get { return _imgHeight; }
            set
            {
                _imgHeight = value;
                OnPropertyChanged();
            }
        }

        public int ImgWidth
        {
            get { return _imgWidth; }
            set
            {
                _imgWidth= value;
                OnPropertyChanged();
            }
        }

        public string Dir
        {
            get { return _dir; }
            set
            {
                _dir = value;
                Files = (from x in Directory.GetFiles(value)
                                  select Path.GetFileName(x)).ToArray();
            }
        }

        public TimeSpan Span
        {
            get { return _span; }
            set
            {
                _span = value;
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

        public double TimeRemaining
        {
            get { return _timeRemaining; }
            set
            {
                _timeRemaining = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public void Execute()
        {
            if (!Initialize())
                return;

            while (!Cancelled)
            {
                int ind = Convert.ToInt32(Math.Round(Rand.NextDouble() * (Max - Min) + Min));
                Action act = new Action(() => 
                {
                    CurrentImg = Path.Combine(Dir, Files[ind]);
                    Image img = Image.FromFile(CurrentImg);
                    int height = img.Height;
                    ImgWidth = (img.Width * ImgHeight) / height;

                });

                Application.Current.Dispatcher.BeginInvoke(act);

                Stopwatch s = new Stopwatch();
                s.Start();
                while (s.Elapsed < Span && !Skipped)
                {
                    TimeRemaining =  ((Span - s.Elapsed).TotalSeconds / TimeTotal)*100;

                }
                s.Stop();
                s.Reset();
                Skipped = false;
            }
           
        }

        private bool Initialize()
        {
            Min = 0;
            Max = Files.Count() - 1;

            if (Max < 1)
                return false;

            Cancelled = false;
            Skipped = false;
            TimeTotal = Span.TotalSeconds;

            return true;
        }

        public void Next()
        {
            Skipped = true;
        }

        public void Cancel()
        {
            Skipped = true;
            Cancelled = true;
        }

        #region Event Handler
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
