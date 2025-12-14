using restaurantManager.Models;
using restaurantManager.Services;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

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
                ViewModels.Admin.menuManager.ReloadDanhSachTheoTab(tabItem.Name);
            }
        }

        private void btnThem_Click(object sender, RoutedEventArgs e)
        {
            // ✅ Validate input
            if (string.IsNullOrWhiteSpace(txtTenMon.Text))
            {
                MessageBox.Show("Vui lòng nhập tên món ăn", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTenMon.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMoTa.Text))
            {
                MessageBox.Show("Vui lòng nhập mô tả", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtMoTa.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtGia.Text) || !decimal.TryParse(txtGia.Text, out decimal gia) || gia <= 0)
            {
                MessageBox.Show("Vui lòng nhập giá hợp lệ (> 0)", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtGia.Focus();
                return;
            }

            if (cbbLoaiMonAn.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn loại món", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                cbbLoaiMonAn.Focus();
                return;
            }

            string imageSourcePath = ViewModels.Admin.menuManager.filePathImage;
            if (string.IsNullOrWhiteSpace(imageSourcePath) || !File.Exists(imageSourcePath))
            {
                MessageBox.Show("Vui lòng chọn ảnh cho món ăn", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ✅ Gọi hàm thêm từ ViewModel
            if (ViewModels.Admin.menuManager.ThemMonAn(
                txtTenMon.Text.Trim(),
                txtMoTa.Text.Trim(),
                gia,
                cbbLoaiMonAn.SelectedIndex,
                imageSourcePath))
            {
                // Clear form
                ClearForm();

                // Reload danh sách
                ReloadListView();
            }
        }

        private void btnSua_Click(object sender, RoutedEventArgs e)
        {
            if (MonAnSelection == null)
            {
                MessageBox.Show("Vui lòng chọn món ăn cần sửa", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // ✅ Validate input
            if (string.IsNullOrWhiteSpace(txtTenMon.Text))
            {
                MessageBox.Show("Vui lòng nhập tên món ăn", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTenMon.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMoTa.Text))
            {
                MessageBox.Show("Vui lòng nhập mô tả", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtMoTa.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtGia.Text) || !decimal.TryParse(txtGia.Text, out decimal gia) || gia <= 0)
            {
                MessageBox.Show("Vui lòng nhập giá hợp lệ (> 0)", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtGia.Focus();
                return;
            }

            if (cbbLoaiMonAn.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn loại món", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                cbbLoaiMonAn.Focus();
                return;
            }

            string imageSourcePath = ViewModels.Admin.menuManager.filePathImage;

            // ✅ Gọi hàm sửa từ ViewModel
            if (ViewModels.Admin.menuManager.SuaMonAn(
                MonAnSelection.MaMonAn,
                txtTenMon.Text.Trim(),
                txtMoTa.Text.Trim(),
                gia,
                cbbLoaiMonAn.SelectedIndex,
                imageSourcePath,
                MonAnSelection.HinhAnhURL))
            {
                // Clear form
                ClearForm();

                // Reload danh sách
                ReloadListView();
            }
        }

        private void btnXoa_Click(object sender, RoutedEventArgs e)
        {
            if (MonAnSelection == null)
            {
                MessageBox.Show("Vui lòng chọn món ăn cần xóa", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Bạn có chắc muốn xóa '{MonAnSelection.TenMonAn}' không?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                if (ViewModels.Admin.menuManager.XoaMonAn(
                    MonAnSelection.MaMonAn,
                    MonAnSelection.HinhAnhURL,
                    MonAnSelection.TenMonAn))
                {
                    // Clear form
                    ClearForm();

                    // Reload danh sách
                    ReloadListView();
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModels.Admin.menuManager.OpenFileImageFood())
            {
                try
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(ViewModels.Admin.menuManager.filePathImage, UriKind.Absolute);
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                    ImageFoodDiaLog.Source = bitmapImage;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi tải ảnh: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    ImageFoodDiaLog.Source = null;
                    ViewModels.Admin.menuManager.filePathImage = null;
                }
            }
        }

        private void lvMonAn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lvMonAn.SelectedItem is MonAn mon)
            {
                MonAnSelection = mon;
                txtTenMon.Text = mon.TenMonAn;
                txtMoTa.Text = mon.MoTa;
                txtGia.Text = mon.Gia.ToString("F0");
                cbbLoaiMonAn.SelectedIndex = mon.Loai;

                // Load ảnh
                LoadImageFromPath(mon.HinhAnhURL);

                // ✅ Clear imageSourcePath khi chọn item
                ViewModels.Admin.menuManager.filePathImage = null;
            }
        }

        // ✅ Hàm load ảnh từ path
        private void LoadImageFromPath(string imagePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(imagePath))
                {
                    ImageFoodDiaLog.Source = null;
                    return;
                }

                // ✅ Kiểm tra xem đường dẫn có phải là tuyệt đối hay tương đối
                string fullPath = imagePath;
                if (!Path.IsPathRooted(imagePath))
                {
                    fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);
                }

                if (!File.Exists(fullPath))
                {
                    ImageFoodDiaLog.Source = null;
                    return;
                }

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(fullPath, UriKind.Absolute);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                ImageFoodDiaLog.Source = bitmapImage;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi load ảnh: {ex.Message}");
                ImageFoodDiaLog.Source = null;
            }
        }

        // ✅ Hàm clear form
        private void ClearForm()
        {
            txtTenMon.Clear();
            txtMoTa.Clear();
            txtGia.Clear();
            cbbLoaiMonAn.SelectedIndex = -1;
            ImageFoodDiaLog.Source = null;
            MonAnSelection = null;
            lvMonAn.SelectedItem = null;
            ViewModels.Admin.menuManager.filePathImage = null;
        }

        // ✅ Hàm reload ListView theo tab hiện tại
        private void ReloadListView()
        {
            if (tabControl.SelectedItem is TabItem tabItem)
            {
                ViewModels.Admin.menuManager.ReloadDanhSachTheoTab(tabItem.Name);
            }
            else
            {
                ViewModels.Admin.menuManager.ReloadDanhSachTheoTab("All");
            }
        }
    }
}