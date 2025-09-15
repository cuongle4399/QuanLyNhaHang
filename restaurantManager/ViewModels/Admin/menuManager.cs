using restaurantManager.Models;
using restaurantManager.Services;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace restaurantManager.ViewModels.Admin
{
    public class menuManager
    {
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

                float gia = 0f;
                float.TryParse(row["Gia"]?.ToString(), out gia);

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
    }
}
