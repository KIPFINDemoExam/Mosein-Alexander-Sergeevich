using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CarService
{
    /// <summary>
    /// Логика взаимодействия для AddEntryWindow.xaml
    /// </summary>
    public partial class AddEntryWindow : Window, INotifyPropertyChanged
    {
        public ServicesInfo servicesInfo { get; set; }
        public AddEntryWindow(ServicesInfo service)
        {
            InitializeComponent();
            servicesInfo = service;
            var sourcecombo = DB.db.Clients.AsNoTracking().Select(it => it.LastName + " " + it.FirstName + " " + it.Patronymic).ToList();
            ClientsCombo.ItemsSource = sourcecombo;
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var time = GetFulltime();
            if (time != default)
            {
                DB.db.ClientServices.Add(new ClientService
                {
                    ClientID = ClientsCombo.SelectedIndex + 1,
                    ServiceID = servicesInfo.ID,
                    StartTime = time
                });
                DB.db.SaveChanges();
            }
            else
            {
                MessageBox.Show("Введено неверное время");
            }
        }
        DateTime GetFulltime()
        {
            var date = (DateTime)SetDate.SelectedDate;
            if (date == null)
                return default;
            DateTime result;
            var checktime = DateTime.TryParse(SetTime.Text, out result);
            if (checktime)
                return date.Add(result.TimeOfDay);
            else
                return default;
        }

        private void SetTime_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txtbox = sender as TextBox;
            var time = txtbox.Text.Split(':');
            int hours = 0;
            int minutes = 0;
            if (!(time.First().Contains("_")))
            {
                hours = int.Parse(time.First());
                if (hours > 23)
                    hours = 00;

                if (hours < 0)
                    hours = 00;
                var timestring = string.Join(":", new int[] { hours, minutes });
                txtbox.Text = timestring;
            }
            if (!(time.Last().Contains("_")))
            {
                minutes = int.Parse(time.Last());

                if (minutes > 59)
                    minutes = 0;

                if (minutes < 0)
                    minutes = 0;
                var timestring = string.Join(":", new int[] { hours, minutes });
                txtbox.Text = timestring;
            }



        }

        private void SetTime_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
           /* var txtbox = sender as TextBox;            
            if (!char.IsDigit(char.Parse(e.Text)))
                e.Handled = true;*/
        }
    }
}
