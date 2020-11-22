using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using VieCommune.Serialization;
using FigureOfSketch.Objects;
using System.IO;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace FigureOfSketch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Configuration config = XmlSerializerExt<Configuration>.DeserializeObject("Config.xml");

            Dir = config.Directory;
            InitializeDirectory();

            GrabBag = new GrabBag();
            this.DataContext = this;
            spContent.DataContext = GrabBag;
        }

        #region Properties
        public string Dir { get; set; }

        public string SelectedDir { get; set; }
        public TimeSpan SelectedSpan { get; set; }

        public GrabBag GrabBag { get; set; }
        public BackgroundWorker Worker { get; set; }

        public List<TimeSpan> Times { get; set; } = 
            new List<TimeSpan>() { new TimeSpan(0, 0, 30), new TimeSpan(0, 1, 0), new TimeSpan(0, 1, 30), new TimeSpan(0, 2, 0), new TimeSpan(0, 5, 30), new TimeSpan(0, 10, 0), new TimeSpan(120,0,0) };

        public ObservableCollection<string> Directories { get; set; } = new ObservableCollection<string>();
        #endregion

        #region Initialization
        private void InitializeDirectory()
        {
            IEnumerable<string> directories = (from x in Directory.GetDirectories(Dir) select Path.GetFileName(x));
            
            foreach(string dir in directories)
            {
                Directories.Add(dir);
            }
        }
        #endregion

        #region Events
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GrabBag.Dir = SelectedDir;
            GrabBag.ImgHeight = 600;
            GrabBag.Span = SelectedSpan;

            btnStart.IsEnabled = false;

            Worker = new BackgroundWorker();
            Worker.DoWork += ExecuteTest;
            Worker.RunWorkerAsync();
        }

        public void ExecuteTest(object sender, DoWorkEventArgs args)
        {
            GrabBag.Execute();
        }
       

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            GrabBag.Next();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            GrabBag.Cancel();
            btnStart.IsEnabled = true;
        }
        #endregion

        private void cmbContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedDir= Path.Combine(Dir, cmbContent.SelectedItem.ToString());
        }

        private void cmbTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedSpan= (TimeSpan)cmbTime.SelectedItem;
        }
    }
}
