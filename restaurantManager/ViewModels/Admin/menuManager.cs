using Microsoft.Win32;
using restaurantManager.Models;
using restaurantManager.Services;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;

namespace restaurantManager.ViewModels.Admin
{
    public class menuManager
    {
        public static string filePathImage = null;
        public static ObservableCollection<MonAn> danhSachMonAn { get; set; } = new ObservableCollection<MonAn>();
        public static ObservableCollection<LoaiMonAn> danhSachLoaiMonAn { get; set; }
            = new ObservableCollection<LoaiMonAn>
            {
                new LoaiMonAn { Text = "Món ăn" },
                new LoaiMonAn { Text = "Nước uống" }
            };

        public static void LoadDanhSachMonAn(string query, params SqlParameter[] parameters)
        {
            try
            {
                danhSachMonAn.Clear();
                DataTable data = DatabaseConnect.ExecuteTable(query, parameters);

                if (data == null || data.Rows.Count == 0)
                    return;

                foreach (DataRow row in data.Rows)
                {
                    int maMonAn = 0;
                    int.TryParse(row["MaMonAn"]?.ToString(), out maMonAn);

                    string tenMonAn = row["TenMonAn"]?.ToString() ?? "";
                    string hinhAnhURL = row["HinhAnhURL"]?.ToString() ?? "";
                    string moTa = row["MoTa"]?.ToString() ?? "";

                    decimal gia = 0;
                    decimal.TryParse(row["Gia"]?.ToString(), out gia);

                    int loaiMonAn = 0;
                    int.TryParse(row["Loai"]?.ToString(), out loaiMonAn);

                    // Xử lý đường dẫn ảnh
                    string fullHinhAnhURL = hinhAnhURL;
                    if (!Path.IsPathRooted(hinhAnhURL))
                    {
                        fullHinhAnhURL = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, hinhAnhURL);
                    }

                    if (!File.Exists(fullHinhAnhURL))
                    {
                        fullHinhAnhURL = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Image", "default.png");
                    }

                    MonAn monAn = new MonAn(maMonAn, tenMonAn, gia, moTa, fullHinhAnhURL, loaiMonAn);
                    danhSachMonAn.Add(monAn);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi LoadDanhSachMonAn: {ex.Message}");
                MessageBox.Show($"Lỗi load dữ liệu: {ex.Message}");
            }
        }

        public static bool OpenFileImageFood()
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = "Image files (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All files (*.*)|*.*",
                    Title = "Chọn ảnh món ăn"
                };
                bool? result = openFileDialog.ShowDialog();
                if (result == true && !string.IsNullOrEmpty(openFileDialog.FileName))
                {
                    filePathImage = openFileDialog.FileName;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở dialog: {ex.Message}");
                return false;
            }
        }

        // ✅ FIX: Sửa cách xử lý SqlParameter cho dish và beverage
        public static void ReloadDanhSachTheoTab(string tabName)
        {
            try
            {
                switch (tabName)
                {
                    case "All":
                        // Không có parameter, load tất cả
                        LoadDanhSachMonAn("SELECT * FROM MonAn ORDER BY MaMonAn DESC");
                        break;
                    case "dish":
                        // ✅ Tạo SqlParameter với kiểu dữ liệu rõ ràng (SqlDbType.Int)
                        SqlParameter paramDish = new SqlParameter("@loai", SqlDbType.Int);
                        paramDish.Value = 0;
                        LoadDanhSachMonAn(
                            "SELECT * FROM MonAn WHERE Loai = @loai ORDER BY MaMonAn DESC",
                            paramDish);
                        break;
                    case "beverage":
                        // ✅ Tạo SqlParameter với kiểu dữ liệu rõ ràng (SqlDbType.Int)
                        SqlParameter paramBeverage = new SqlParameter("@loai", SqlDbType.Int);
                        paramBeverage.Value = 1;
                        LoadDanhSachMonAn(
                            "SELECT * FROM MonAn WHERE Loai = @loai ORDER BY MaMonAn DESC",
                            paramBeverage);
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi ReloadDanhSachTheoTab: {ex.Message}");
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }

        public static string SaveImageToDisk(string sourceImagePath, int loai)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sourceImagePath) || !File.Exists(sourceImagePath))
                {
                    return null;
                }

                string folderName = loai == 0 ? "dish" : "beverage";
                string pathfolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "Resources", "Image", folderName);

                if (!Directory.Exists(pathfolder))
                {
                    Directory.CreateDirectory(pathfolder);
                }

                string nameFile = Path.GetFileName(sourceImagePath);
                string pathSave = Path.Combine(pathfolder, nameFile);

                if (File.Exists(pathSave))
                {
                    string fileNameWithoutExt = Path.GetFileNameWithoutExtension(nameFile);
                    string extension = Path.GetExtension(nameFile);
                    nameFile = $"{fileNameWithoutExt}_{DateTime.Now:yyyyMMdd_HHmmss}{extension}";
                    pathSave = Path.Combine(pathfolder, nameFile);
                }

                File.Copy(sourceImagePath, pathSave, true);

                return Path.Combine("Resources", "Image", folderName, nameFile).Replace("\\", "\\");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu ảnh: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public static bool ThemMonAn(string tenMonAn, string moTa, decimal gia, int loai, string imageSourcePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenMonAn) || string.IsNullOrWhiteSpace(moTa) ||
                    gia <= 0 || string.IsNullOrWhiteSpace(imageSourcePath))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                if (!File.Exists(imageSourcePath))
                {
                    MessageBox.Show("File ảnh không tồn tại", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                string relativePath = SaveImageToDisk(imageSourcePath, loai);
                if (string.IsNullOrEmpty(relativePath))
                {
                    MessageBox.Show("Không thể lưu ảnh", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                bool result = DatabaseConnect.ExecuteNonQuery(
                    "INSERT INTO MonAn (TenMonAn, HinhAnhURL, MoTa, Gia, Loai) VALUES (@TenMonAn, @HinhAnhURL, @MoTa, @Gia, @Loai)",
                    new SqlParameter("@TenMonAn", tenMonAn),
                    new SqlParameter("@HinhAnhURL", relativePath),
                    new SqlParameter("@MoTa", moTa),
                    new SqlParameter("@Gia", gia),
                    new SqlParameter("@Loai", loai));

                if (result)
                {
                    MessageBox.Show("Đã thêm món vào thực đơn", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    filePathImage = null;
                }
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static bool SuaMonAn(int maMonAn, string tenMonAn, string moTa, decimal gia, int loai, string imageSourcePath, string oldImagePath)
        {
            try
            {
                if (maMonAn <= 0 || string.IsNullOrWhiteSpace(tenMonAn) ||
                    string.IsNullOrWhiteSpace(moTa) || gia <= 0)
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                string imagePath = oldImagePath ?? "";

                if (!string.IsNullOrWhiteSpace(imageSourcePath) && File.Exists(imageSourcePath))
                {
                    imagePath = SaveImageToDisk(imageSourcePath, loai);
                    if (string.IsNullOrEmpty(imagePath))
                    {
                        MessageBox.Show("Không thể lưu ảnh", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }

                    try
                    {
                        if (!string.IsNullOrWhiteSpace(oldImagePath))
                        {
                            string fullOldPath = oldImagePath;
                            if (!Path.IsPathRooted(oldImagePath))
                            {
                                fullOldPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, oldImagePath);
                            }

                            if (File.Exists(fullOldPath) && fullOldPath != imageSourcePath)
                            {
                                File.Delete(fullOldPath);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Lỗi xóa ảnh cũ: {ex.Message}");
                    }
                }

                bool result = DatabaseConnect.ExecuteNonQuery(
                    "UPDATE MonAn SET TenMonAn = @TenMonAn, MoTa = @MoTa, Gia = @Gia, Loai = @Loai, HinhAnhURL = @HinhAnhURL WHERE MaMonAn = @MaMonAn",
                    new SqlParameter("@MaMonAn", maMonAn),
                    new SqlParameter("@TenMonAn", tenMonAn),
                    new SqlParameter("@MoTa", moTa),
                    new SqlParameter("@Gia", gia),
                    new SqlParameter("@Loai", loai),
                    new SqlParameter("@HinhAnhURL", imagePath));

                if (result)
                {
                    MessageBox.Show("Đã cập nhập món ăn thành công", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    filePathImage = null;
                }
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static bool XoaMonAn(int maMonAn, string imagePath, string tenMonAn)
        {
            try
            {
                if (maMonAn <= 0)
                {
                    MessageBox.Show("Vui lòng chọn món ăn cần xóa", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                bool result = DatabaseConnect.ExecuteNonQuery(
                    "DELETE FROM MonAn WHERE MaMonAn = @MaMonAn",
                    new SqlParameter("@MaMonAn", maMonAn));

                if (result)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(imagePath))
                        {
                            string fullPath = imagePath;
                            if (!Path.IsPathRooted(imagePath))
                            {
                                fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imagePath);
                            }

                            if (File.Exists(fullPath))
                            {
                                File.Delete(fullPath);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Lỗi xóa ảnh: {ex.Message}");
                    }

                    MessageBox.Show($"Đã xóa thành công {tenMonAn}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    filePathImage = null;
                }
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static void ClearFormData()
        {
            filePathImage = null;
        }
    }
}