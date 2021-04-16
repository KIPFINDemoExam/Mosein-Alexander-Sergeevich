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

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            var time = GetFulltime();
            if (ClientsCombo.SelectedIndex != -1)
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
            else
                MessageBox.Show("Не выбран клиент!");

        }
        DateTime GetFulltime()
        {
            if (SetDate.SelectedDate != default)
            {
                var date = (DateTime)SetDate.SelectedDate;
                if (date == null)
                    return default;
                DateTime result;
                var checktime = DateTime.TryParse($"{Hours.Text}:{Minutes.Text}", out result);
                if (checktime)
                    return date.Add(result.TimeOfDay);
                else
                    return default;
            }
            else
                return default;
        }

        private void Hours_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txtbox = sender as TextBox;
            if (string.IsNullOrEmpty(txtbox.Text))
                return;
            var time = int.Parse(txtbox.Text);
            if (time > 23)
                txtbox.Text = "00";
            if (time < 0)
                txtbox.Text = "00";
        }

        private void Minutes_TextChanged(object sender, TextChangedEventArgs e)
        {

            var txtbox = sender as TextBox;
            if (string.IsNullOrEmpty(txtbox.Text))
                return;
            var time = int.Parse(txtbox.Text);
            if (time > 59)
                txtbox.Text = "00";
            if (time < 0)
                txtbox.Text = "00";
        }

        private void CheckInt(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!(char.IsDigit(e.Text.First())) && string.IsNullOrEmpty(e.Text))
                e.Handled = true;
        }
    }
}
