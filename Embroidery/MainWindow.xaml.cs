using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
using Converter;


namespace Embroidery
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int GridItemSize
        {
            get
            {
                int res = 0;
                if (Int32.TryParse(tbGridItemSize.Text, out res))
                    return res;

                return 10;
            }

        }
        public static Bitmap _Image;
        Converter.ImageConverter _ImageConverter;

        public MainWindow()
        {
            InitializeComponent();
            _ImageConverter = Converter.ImageConverter.getInstance(GridItemSize);
        }

        private void btnSelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FileOk += OpenFileDialog_FileOk;
            openFileDialog.ShowDialog();
        }

        private void OpenFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                _Image = new Bitmap((sender as OpenFileDialog).FileName);
                // DrawImage(_ImageConverter.ConvertImage(_Image));
                DrawImage(_ImageConverter.ConvertImage(_ImageConverter.AlignColor(_Image)));
            }
            catch
            {
                MessageBox.Show("Ошибка! Невозможно считать файл.");
            }
        }

        public void DrawImage(Bitmap image)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                image.Save(memory, ImageFormat.Jpeg);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                imgImage.Source = bitmapImage;
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (_Image != null)
            {
                _ImageConverter.GridItemSize = GridItemSize;
                // DrawImage(_ImageConverter.ConvertImage(_Image));
                DrawImage(_ImageConverter.ConvertImage(_ImageConverter.AlignColor(_Image)));
            }
            else
            {
                MessageBox.Show("Картинка не загружена! ");
            }
        }
    }
}
