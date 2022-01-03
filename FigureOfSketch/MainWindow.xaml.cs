using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using FigureOfSketch.Objects;
using System.IO;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace FigureOfSketch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        private string _baseDirectory;

        public MainWindow()
        {
            BaseDirectory = Configuration.Current.Directory;

            InitializeComponent();            
            InitializeDirectory();

            GrabBag = new GrabBag();

            this.DataContext = this;
            gridContent.DataContext = GrabBag;
        }

        

        #region Initialization
        private void InitializeDirectory()
        {
            if(cmbContent!=null)
                cmbContent.SelectedIndex = -1;

            Directories.Clear();
            IEnumerable<string> directories = (from x in Directory.GetDirectories(BaseDirectory) select Path.GetFileName(x));

            foreach (string dir in directories)
            {
                Directories.Add(dir);
            }
            if (cmbContent != null)
                cmbContent.SelectedIndex = 0;
        }
        #endregion



        #region Properties
        public string BaseDirectory 
        {
            get { return _baseDirectory; }
            set
            {
                _baseDirectory = value;
                Configuration.Current.Directory = _baseDirectory;
                Configuration.Save();
                InitializeDirectory();
                OnPropertyChanged();
            }
        }
        public string ActiveDirectory { get; set; }

        public GrabBag GrabBag { get; set; }
        public BackgroundWorker Worker { get; set; }

        public ObservableCollection<string> Directories { get; set; } = new ObservableCollection<string>();
        #endregion


        #region Functionality Events

        public void ExecuteTest(object sender, DoWorkEventArgs args)
        {
            GrabBag.Execute();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (GrabBag.IsRunning)
                GrabBag.Unpause();
            else
            {
                GrabBag.ActiveDirectory = ActiveDirectory;
                GrabBag.Span = new TimeSpan(0, 0, Convert.ToInt16(timeSlider.Value));

                Worker = new BackgroundWorker();
                Worker.DoWork += ExecuteTest;
                Worker.RunWorkerAsync();
            }
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            GrabBag.Pause();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            GrabBag.Next();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            GrabBag.Cancel();
        }
        #endregion

        #region UI Events
        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            if (GrabBag.IsRunning)
                GrabBag.Cancel();
            this.Close();
        }

        private void buttonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void buttonMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void tabFileControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            TabItem tab = sender as TabItem;

            if (tab.IsSelected == true)
                tab.IsSelected = false;
            else
                tab.IsSelected = true;
        }

        private void cmbContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cmbContent.SelectedItem != null)
                ActiveDirectory = Path.Combine(BaseDirectory, cmbContent.SelectedItem.ToString());
        }

        private void btnOpenFileExplorer_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
            {
                BaseDirectory = dialog.SelectedPath;
            }
        }
        #endregion

        #region NotifyProperty
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        #endregion

        
    }
}
