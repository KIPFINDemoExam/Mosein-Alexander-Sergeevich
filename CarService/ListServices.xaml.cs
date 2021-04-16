using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CarService
{
    public enum SortingDataGrid
    {
        ByDiscount = 1,
        ByPriceAscending,
        ByPriceDescending,
        ByName
    }
    /// <summary>
    /// Логика взаимодействия для ListServices.xaml
    /// </summary>
    public partial class ListServices : Page
    {
        public static bool IsAdmin { get; set; }
        public static List<ServicesInfo> services { get; set; }
        public static int[] CountServices { get; set; }
        public ListServices()
        {
            InitializeComponent();
            services = new List<ServicesInfo>();
            //if (DB.CurrentEmployee.Роль == "Администратор")
            //    IsAdmin = true;
            //else
            //    IsAdmin = false;
            RewriteDataGrid(this);
            var discounts = new List<string>
            {
                "от 0 до 5%",
                "от 5 до 15%",
                "от 15 до 30%",
                "от 30 до 70%",
                "от 70 до 100%",
            };
            discounts.ForEach(it => SortByDiscountCombo.Items.Add(it));
            TotalCount.Text = services.Count.ToString();
            CurrentCount.Text = TotalCount.Text;
            DataContext = this;
        }

        private void DataGridService_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            (sender as DataGrid).UnselectAllCells();
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            Filter();
        }


        private void SortByDiscountCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Filter();
        }

        private void SortByName_TextChanged(object sender, TextChangedEventArgs e)
        {
            Filter();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Application.Current.Windows.OfType<EditWindow>().Any())
            {
                ServicesInfo item = DataGridService.SelectedItems[0] as ServicesInfo;
                var edititem = DB.db.Services.Where(it => it.ID == item.ID).First();
                var window = new EditWindow(edititem, AddEditMode.Edit, this);
                window.Show();
            }
            else
                MessageBox.Show("Окно Добавления/Редактирования уже открыто!", "Ошибка");

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var respond = MessageBox.Show("Вы действительно хотите удалить данную услугу?", "Удаление", MessageBoxButton.YesNo);
            if (respond == MessageBoxResult.Yes)
            {
                ServicesInfo item = DataGridService.SelectedItems[0] as ServicesInfo;
                var removeitem = DB.db.Services.Where(it => it.ID == item.ID).First();
                if (removeitem.ClientServices.Any())
                {
                    MessageBox.Show("На данную услугу имеются записи");
                    return;
                }
                var countPhoto = removeitem.ServicePhotoes.Count;
                for (int i = 0; i < countPhoto; i++)
                {
                    DB.db.ServicePhotoes.Remove(removeitem.ServicePhotoes.First());
                }
                DB.db.SaveChanges();
                DB.db.Services.Remove(removeitem);
                DB.db.SaveChanges();
                RewriteDataGrid(this);
            }
        }

        private void AddService_Click(object sender, RoutedEventArgs e)
        {
            if (!Application.Current.Windows.OfType<EditWindow>().Any())
            {
                var window = new EditWindow(new Service(), AddEditMode.Add, this);
                window.Show();
            }
            else
                MessageBox.Show("Окно Добавления/Редактирования уже открыто!", "Ошибка");
        }

        public void Filter()
        {



            DataGridService.ItemsSource = GetDisountFilter(GetSortFilter(GetByName()));
            CurrentCount.Text = DataGridService.Items.Count.ToString();
        }

        IEnumerable<ServicesInfo> GetSortFilter(IEnumerable<ServicesInfo> currentsort)
        {
            if ((bool)AscendingRadio.IsChecked)
                currentsort = currentsort.OrderBy(it => it.CostWithDiscount);
            else if ((bool)DescendingRadio.IsChecked)
                currentsort = currentsort.OrderByDescending(it => it.CostWithDiscount);
            return currentsort;
        }
        IEnumerable<ServicesInfo> GetDisountFilter(IEnumerable<ServicesInfo> currentsort)
        {

            if (SortByDiscountCombo.SelectedItem != null)
            {
                int intnum;
                var discountrange = SortByDiscountCombo.SelectedItem.ToString().Split().Select(it => it.Trim('%')).Where(it => int.TryParse(it, out intnum)).Select(it => int.Parse(it));
                var lowerthreshold = discountrange.First();
                var highthreshold = discountrange.ToList()[1];
                currentsort = currentsort.Where(it => it.Discount >= lowerthreshold && it.Discount < highthreshold);
            }
            return currentsort;
        }
        IEnumerable<ServicesInfo> GetByName()
        {
            return services.Where(it =>
            {
                if (!string.IsNullOrEmpty(SortByName.Text))
                    return it.Title.ToUpper().StartsWith(SortByName.Text.ToUpper());
                else
                    return it.Equals(it);
            });
        }

        private void AddEntryButton_Click(object sender, RoutedEventArgs e)
        {
            ServicesInfo item = DataGridService.SelectedItems[0] as ServicesInfo;
            var edititem = DB.db.Services.Where(it => it.ID == item.ID).First();
            var window = new AddEntryWindow(item);
            window.Show();
        }
        public static void RewriteDataGrid(ListServices page)
        {
            services = new List<ServicesInfo>();
            DB.db.Services.ToList().ForEach(it =>

                services.Add(new ServicesInfo(it.ID, it.Title, (double)it.Cost, it.DurationInSeconds, (double)it.Discount, it.MainImagePath)
            ));
            page.DataGridService.ItemsSource = services;
            page.TotalCount.Text = services.Count.ToString();
            page.Filter();
        }

        private void RemoveFilter_Click(object sender, RoutedEventArgs e)
        {
            DataGridService.ItemsSource = services;
            SortByDiscountCombo.SelectedItem = null;
            AscendingRadio.IsChecked = false;
            DescendingRadio.IsChecked = false;
            SortByName.Text = null;
        }

        private void GoToNearestsEntries(object sender, RoutedEventArgs e)
        {
            var nearestsentrieswindow = new NearestsEntriesWindow();
            nearestsentrieswindow.ShowDialog();
        }
    }
}
