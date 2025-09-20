using restaurantManager.Models;
using restaurantManager.Services;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.IO;

namespace restaurantManager.View.Fuction.Admin
{
    public partial class menuManager : UserControl
    {
        public ObservableCollection<MonAn> DanhSachMonAn { get; set; }
        public ObservableCollection<LoaiMonAn> DanhSachLoaiMonAn { get; set; }
        public MonAn MonAnSelection { get; set; }
        public menuManager()
        {
            InitializeComponent();

            ViewModels.Admin.menuManager.LoadDanhSachMonAn("SELECT * FROM MonAn");
            DanhSachMonAn = ViewModels.Admin.menuManager.danhSachMonAn;
            DanhSachLoaiMonAn = ViewModels.Admin.menuManager.danhSachLoaiMonAn;
            this.DataContext = this;
        }

        private void TabItem_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is TabItem tabItem)
            {
                switch (tabItem.Name)
                {
                    case "All":
                        ViewModels.Admin.menuManager.LoadDanhSachMonAn("SELECT * FROM MonAn");
                        break;
                    case "dish":
                        ViewModels.Admin.menuManager.LoadDanhSachMonAn("SELECT * FROM MonAn WHERE Loai = @loai",new SqlParameter ("@loai","0"));
                        break;
                    case "beverage":
                        ViewModels.Admin.menuManager.LoadDanhSachMonAn("SELECT * FROM MonAn WHERE Loai = @loai", new SqlParameter("@loai", "1"));
                        break;
                }
            }
        }

        private void btnThem_Click(object sender, RoutedEventArgs e)
        {
            string pathImage = ViewModels.Admin.menuManager.filePathImage;
            if (string.IsNullOrEmpty(txtTenMon.Text) || string.IsNullOrEmpty(txtGia.Text) || string.IsNullOrEmpty(txtMoTa.Text) ||
                ImageFoodDiaLog.Source == null || string.IsNullOrEmpty(pathImage)) 
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin");
                return;
            }
            if(DatabaseConnect.ExecuteNonQuery("INSERT INTO MonAn (TenMonAn, HinhAnhURL, MoTa,Gia, Loai) VALUES (@TenMonAn, @HinhAnhURL, @MoTa, @Gia, @Loai)",
              new SqlParameter("@TenMonAn", txtTenMon.Text), new SqlParameter("@HinhAnhURL", pathImage), new SqlParameter("@MoTa", txtMoTa.Text), new SqlParameter("@Gia", decimal.Parse(txtGia.Text)), new SqlParameter("@Loai", cbbLoaiMonAn.SelectedIndex)))
            {
                string pathfolder;
                if (cbbLoaiMonAn.SelectedIndex == 0)
                {
                    pathfolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\Image\\dish");
                }
                else
                {
                    pathfolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\Image\\beverage");
                }
                if (!Directory.Exists(pathfolder))
                {
                    Directory.CreateDirectory(pathfolder);
                }
                string nameFile = Path.GetFileName(pathImage);
                string pathSave = Path.Combine(pathfolder, nameFile);
                File.Copy(pathImage, pathSave, true);
                ViewModels.Admin.menuManager.filePathImage = null;
                ImageFoodDiaLog.Source = null;
                MessageBox.Show("Đã thêm món vào thực đơn");
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModels.Admin.menuManager.OpenFileImageFood();
            BitmapImage bitmapImage = new BitmapImage(new System.Uri(ViewModels.Admin.menuManager.filePathImage));
            ImageFoodDiaLog.Source = bitmapImage;
        }

        private void lvMonAn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lvMonAn.SelectedItem is MonAn mon)
            {
                this.MonAnSelection = mon;
                txtTenMon.Text = mon.TenMonAn;
                txtMoTa.Text = mon.MoTa;
                txtGia.Text = mon.Gia.ToString();
                cbbLoaiMonAn.SelectedIndex = mon.Loai;
                if (!string.IsNullOrWhiteSpace(mon.HinhAnhURL) && File.Exists(mon.HinhAnhURL))
                {
                    try
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri(mon.HinhAnhURL, UriKind.Absolute);
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        ImageFoodDiaLog.Source = bitmapImage;
                    }
                    catch
                    {
                        ImageFoodDiaLog.Source = null; 
                    }
                }
                else
                {
                    ImageFoodDiaLog.Source = null;
                }


            }
        }

        private void btnSua_Click(object sender, RoutedEventArgs e)
        {
            if (this.MonAnSelection == null)
            {
                MessageBox.Show("Vui lòng chọn món ăn cần sửa");
                return;
            }
           string pathImage = ViewModels.Admin.menuManager.filePathImage
                       ?? this.MonAnSelection.HinhAnhURL
                       ?? "";
            if (DatabaseConnect.ExecuteNonQuery("UPDATE MonAn SET TenMonAn = @TenMonAn," +
                "MoTa = @moTa, Gia = @gia, Loai = @loai, HinhAnhURL = @hinhAnhUrl" +
                " WHERE @maMonAn = MaMonAn", 
                new SqlParameter("@maMonAn", this.MonAnSelection.MaMonAn),
                new SqlParameter("@TenMonAn", txtTenMon.Text),
                new SqlParameter("@HinhAnhURL", pathImage),
                new SqlParameter("@MoTa", txtMoTa.Text),
                new SqlParameter("@Gia", decimal.Parse(txtGia.Text)),
                new SqlParameter("@Loai", cbbLoaiMonAn.SelectedIndex)))
            {
                MessageBox.Show($"Đã cập nhập món ăn thành công");
            }
            ViewModels.Admin.menuManager.LoadDanhSachMonAn("SELECT * FROM MonAn");
            ViewModels.Admin.menuManager.filePathImage = null;
        }

        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (this.MonAnSelection == null)
            {
                MessageBox.Show("Vui lòng chọn món ăn cần xóa");
                return;
            }
            if (DatabaseConnect.ExecuteNonQuery("DELETE FROM MonAn WHERE @maMonAn = MaMonAn", new SqlParameter("@maMonAn", this.MonAnSelection.MaMonAn)))
            {
                MessageBox.Show($"Đã xóa thành công {this.MonAnSelection.TenMonAn} ra khỏi menu món ăn");
            }
            ViewModels.Admin.menuManager.LoadDanhSachMonAn("SELECT * FROM MonAn");
            txtTenMon.Text = txtGia.Text = txtMoTa.Text = "";
            ImageFoodDiaLog.Source = null;
        }
    }
}