using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CarService
{
    public enum AddEditMode
    {
        Add = 1,
        Edit
    }

    public enum FileDialogImage
    {
        AddExtra = 1,
        EditMain,
        EditExtra
    }

    /// <summary>
    /// Логика взаимодействия для EditPage.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        public class ExtraImage
        {
            public int ID { get; set; }
            public string Path { get; set; }
            public Image Photo { get; set; }
            public ExtraImage(int id, string path)
            {
                ID = id;
                Path = path;
                Photo = new Image() { Source = new BitmapImage(new Uri(path, UriKind.Relative)) };
            }
        }

        public List<ExtraImage> Images { get; set; } // дополнительные изображения этого элемента
        public List<ServicePhoto> ExtraPhotoes { get; set; } // дополнительные изображения всех элементов
        ExtraImage clickableimage;
        public static AddEditMode Mode { get; set; }
        public static Service service { get; set; }
        ListServices Pageservices { get; set; } // главная страница
        public EditWindow(Service editservice, AddEditMode mode, ListServices listServices)
        {
            InitializeComponent();
            service = editservice;
            Mode = mode;
            ExtraPhotoes = DB.db.ServicePhotoes.ToList();
            Images = new List<ExtraImage>();
            Pageservices = listServices;
            AddExtraPhotoes();
            DataContext = this;
        }
        void AddExtraPhotoes()
        {
            if (ExtraImages.Children.Count != 0)
                ExtraImages.Children.Clear();
            if (ExtraPhotoes.Any())
            {
                var exists = ExtraPhotoes.Any(it => it.ServiceID == service.ID);
                if (exists)
                {
                    Images = ExtraPhotoes.Where(it => it.ServiceID == service.ID)
                    .Select(it => new ExtraImage(it.ServiceID, it.PhotoPath)).ToList();
                    Images.ForEach(it =>
                    {
                        var delete = new MenuItem() { Header = "Удалить" };
                        delete.Click += Delete_Click;
                        it.Photo.ContextMenu = new ContextMenu();
                        it.Photo.ContextMenu.Items.Add(delete);
                        ExtraImages.Children.Add(it.Photo);
                        it.Photo.MouseRightButtonDown += Image_MouseRightButtonDown;
                    });
                    if (Mode == AddEditMode.Add)
                        IDText.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void Image_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            
            clickableimage = Images.Where(it => it.Photo == sender as Image).First();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var element = ExtraPhotoes.Where(it => it.PhotoPath == clickableimage.Path && it.ServiceID == clickableimage.ID).First();
            ExtraPhotoes.Remove(element);
            Images.Remove(clickableimage);
            AddExtraPhotoes();
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            OpenImageFromDialog(FileDialogImage.EditExtra);
            AddExtraPhotoes();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this;
        }
        private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenImageFromDialog(FileDialogImage.EditMain);

        }
        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if (Mode == AddEditMode.Edit)
            {
                if (!(DB.db.Services.Where(it => it.Title.ToUpper() == service.Title.ToUpper()).Count() <= 1))
                {
                    MessageBox.Show("Услуга с таким названием уже существует!", "Ошибка");
                    return;
                }
            }
            else
            {
                if (DB.db.Services.All(it => it.Title.ToUpper() != service.Title.ToUpper()))
                    DB.db.Services.Add(service);
                else
                {
                    MessageBox.Show("Услуга с таким названием уже существует!", "Ошибка");
                    return;
                }
            }
            if (ExtraPhotoes != null)
            {
                DB.db.ServicePhotoes.RemoveRange(DB.db.ServicePhotoes.ToList());
                DB.db.SaveChanges();
                DB.db.ServicePhotoes.AddRange(ExtraPhotoes);
                DB.db.SaveChanges();
                ListServices.services = new List<ServicesInfo>();
                ListServices.RewriteDataGrid(Pageservices);
                ExtraPhotoes = DB.db.ServicePhotoes.ToList();
            }
        }
        public static Brush ConvertToBrush(string brush) => (Brush)new BrushConverter().ConvertFromString(brush);
        private void AddExtraImages_Click(object sender, RoutedEventArgs e)
        {
            OpenImageFromDialog(FileDialogImage.AddExtra);
        }
        void OpenImageFromDialog(FileDialogImage img)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*";
            fd.InitialDirectory = DB.PhotoDirectory.FullName;
            var existphoto = fd.ShowDialog();
            if (existphoto.HasValue)
            {
                var photopath = $@"{DB.PhotoDirectory.Name}\{new FileInfo(fd.FileName).Name}";
                var uriphoto = new Uri(photopath, UriKind.Relative);
                var imagesource = new BitmapImage(uriphoto);
                switch (img)
                {
                    case FileDialogImage.AddExtra:
                        ExtraPhotoes.Add(new ServicePhoto()
                        {
                            ServiceID = service.ID,
                            PhotoPath = photopath,
                        });
                        Images.Add(new ExtraImage(ExtraPhotoes.Last().ServiceID, photopath));
                        ; break;
                    case FileDialogImage.EditExtra:
                        var imgedit = GetImageByID(service.ID);
                        ExtraPhotoes.Where(it => it.PhotoPath == imgedit.Path && it.ID == imgedit.ID).First().PhotoPath = photopath;
                        clickableimage.Photo.Source = imagesource;
                        ; break;
                    case FileDialogImage.EditMain:
                        if ((bool)existphoto)
                            ServiceImage.Source = imagesource;
                        ; break;
                }
            }
            AddExtraPhotoes();
        }
        ExtraImage GetImageByID(int ID) => Images.Where(it => it.ID == ID).First();
    }
}
