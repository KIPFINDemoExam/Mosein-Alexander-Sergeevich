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
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }
        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxLogin.Text) || string.IsNullOrEmpty(Pass.Password))
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            using (var db = new CarEntities())
            {
                var employee = db.Employees.FirstOrDefault(it => it.Логин == TextBoxLogin.Text && it.Пароль == Pass.Password);
                if (employee == null)
                {
                    MessageBox.Show("Пользователь с таким именем не найден");
                }

                else
                {
                    MessageBox.Show("Пользователь успешно найден");
                    DB.CurrentEmployee = employee;
                    NavigationService?.Navigate(new ListServices());

                }

            }
        }

    }
}
