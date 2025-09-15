using restaurantManager.Models;
using restaurantManager.Services;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace restaurantManager.View.Fuction.Admin
{
    public partial class menuManager : UserControl
    {

        public ObservableCollection<MonAn> DanhSachMonAn { get; set; }
        public ObservableCollection<LoaiMonAn> DanhSachLoaiMonAn { get; set; }
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
            //DatabaseConnect.ExecuteNonQuery("INSERT INTO MonAn (TenMonAn, HinhAnhURL, MoTa,Gia, Loai) VALUES (@TenMonAn, @HinhAnhURL, @MoTa, @Gia, @Loai",
            //  new SqlParameter("@TenMonAn",txtTenMon.Text), new SqlParameter("@HinhAnhURL", txtHinh), new SqlParameter("", ""), new SqlParameter("", ""), new SqlParameter("", ""), new SqlParameter("", ""))
        }
    }
}