using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CarService
{
    /// <summary>
    /// Логика взаимодействия для NearestsEntries.xaml
    /// </summary>
    public partial class NearestsEntriesWindow : Window
    {
        public ObservableCollection<ClientService> Entries { get; set; }
        public Timer TimerRefreshEntries { get; set; }

        public NearestsEntriesWindow()
        {
            InitializeComponent();
            TimerStart();
            GetEntries();
            DataContext = this;
        }

        private void TimerStart()
        {
            TimerRefreshEntries = new Timer
            {
                Interval = TimeSpan.FromSeconds(30).TotalMilliseconds
            };
            TimerRefreshEntries.Elapsed += TimerRefreshEntries_Elapsed;
            TimerRefreshEntries.Start();
        }

        private void TimerRefreshEntries_Elapsed(object sender, ElapsedEventArgs e)
        {
            GetEntries();
        }

        private void GetEntries()
        {
            DB.db = new CarEntities();
            Entries = DB.db.ClientServices.ToList().Where(it => it.StartTime.Subtract(DateTime.Now).Days < 2 && it.StartTime.Subtract(DateTime.Now).Seconds >= 0).ToObservableCollection();
        }
    }
}
