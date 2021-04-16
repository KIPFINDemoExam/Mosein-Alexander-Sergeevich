using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CarService
{
    /// <summary>
    /// Логика взаимодействия для StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        public StartPage()
        {
            InitializeComponent();
        }

        private void UserMode(object sender, RoutedEventArgs e)
        {
           ListServices.IsAdmin = false;
            NavigationService?.Navigate(new ListServices());
        }

        private void AdminMode(object sender, RoutedEventArgs e)
        {
            ListServices.IsAdmin = true;
            NavigationService?.Navigate(new ListServices());

        }
    }
}
