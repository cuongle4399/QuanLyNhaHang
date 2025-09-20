using Microsoft.Win32;
using restaurantManager.Models;
using restaurantManager.Services;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using static MaterialDesignThemes.Wpf.Theme;

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
            danhSachMonAn.Clear();

            DataTable data = DatabaseConnect.ExecuteTable(query, parameters);

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
                string fullHinhAnhURL = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, hinhAnhURL);

                if (!File.Exists(fullHinhAnhURL))
                {
                    fullHinhAnhURL = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Image", "default.png");
                }

                MonAn monAn = new MonAn(maMonAn, tenMonAn, gia, moTa, fullHinhAnhURL, loaiMonAn);

                danhSachMonAn.Add(monAn);
            }
        }
        public static void OpenFileImageFood()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg)|*.png;*.jpg|All files (*.*)|*.*";
            openFileDialog.Title = "Chọn ảnh món ăn";
            if(openFileDialog.ShowDialog() == true)
            {
                filePathImage = openFileDialog.FileName;
            }
        }
        public static void ReloadDanhSachTheoTab(string tabName)
        {
            switch (tabName)
            {
                case "All":
                    LoadDanhSachMonAn("SELECT * FROM MonAn");
                    break;

                case "dish":
                    LoadDanhSachMonAn(
                        "SELECT * FROM MonAn WHERE Loai = @loai",
                        new SqlParameter("@loai", "0"));
                    break;

                case "beverage":
                    LoadDanhSachMonAn(
                        "SELECT * FROM MonAn WHERE Loai = @loai",
                        new SqlParameter("@loai", "1"));
                    break;
            }
        }
    }
}
