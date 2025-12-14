using restaurantManager.Models;
using restaurantManager.ViewModel;
using restaurantManager.ViewModels;
using restaurantManager.View;
using restaurantManager.Services;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;

namespace restaurantManager.ViewModels.Admin
{
    public class SaleManagerViewModel : BaseViewModel
    {
        private KhuyenMai _selectedPromotion;
        private string _searchText;
        private ObservableCollection<KhuyenMai> _promotions;

        public ObservableCollection<KhuyenMai> Promotions
        {
            get => _promotions;
            set { _promotions = value; OnPropertyChanged(); }
        }

        public KhuyenMai SelectedPromotion
        {
            get => _selectedPromotion;
            set { _selectedPromotion = value; OnPropertyChanged(); }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                // Gọi SearchPromotions thay vì LoadPromotions
                SearchPromotions();
            }
        }

        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        public SaleManagerViewModel()
        {
            Promotions = new ObservableCollection<KhuyenMai>();
            AddCommand = new RelayCommand(_ => AddPromotion());
            EditCommand = new RelayCommand(param => EditPromotion(param as KhuyenMai));
            DeleteCommand = new RelayCommand(param => DeletePromotion(param as KhuyenMai));

            LoadPromotions();
        }

        private void LoadPromotions()
        {
            try
            {
                Promotions.Clear();
                string sql = @"
                    SELECT 
                        MaKhuyenMai,
                        TenChuongTrinh,
                        MaCode,
                        LoaiGiamGia,
                        GiaTriGiam,
                        NgayBatDau,
                        NgayKetThuc,
                        TrangThai,
                        SoLuotSuDungToiDa,
                        SoLuotDaSuDung
                    FROM KhuyenMai
                    ORDER BY NgayBatDau DESC";

                DataTable dt = DatabaseConnect.ExecuteTable(sql);
                foreach (DataRow row in dt.Rows)
                {
                    Promotions.Add(new KhuyenMai
                    {
                        MaKhuyenMai = (int)row["MaKhuyenMai"],
                        TenChuongTrinh = row["TenChuongTrinh"].ToString(),
                        MaCode = row["MaCode"].ToString(),
                        LoaiGiamGia = row["LoaiGiamGia"].ToString(),
                        GiaTriGiam = (decimal)row["GiaTriGiam"],
                        NgayBatDau = (DateTime)row["NgayBatDau"],
                        NgayKetThuc = (DateTime)row["NgayKetThuc"],
                        TrangThai = row["TrangThai"].ToString(),
                        SoLuotSuDungToiDa = row["SoLuotSuDungToiDa"] == DBNull.Value ? null : (int?)row["SoLuotSuDungToiDa"],
                        SoLuotDaSuDung = (int)row["SoLuotDaSuDung"]
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        private void SearchPromotions()
        {
            // Nếu SearchText rỗng, tải lại tất cả
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadPromotions();
                return;
            }

            try
            {
                Promotions.Clear();
                string sql = @"
                    SELECT 
                        MaKhuyenMai,
                        TenChuongTrinh,
                        MaCode,
                        LoaiGiamGia,
                        GiaTriGiam,
                        NgayBatDau,
                        NgayKetThuc,
                        TrangThai,
                        SoLuotSuDungToiDa,
                        SoLuotDaSuDung
                    FROM KhuyenMai
                    WHERE MaCode LIKE @search OR TenChuongTrinh LIKE @search
                    ORDER BY NgayBatDau DESC";

                var param = new SqlParameter("@search", $"%{SearchText}%");
                DataTable dt = DatabaseConnect.ExecuteTable(sql, param);

                foreach (DataRow row in dt.Rows)
                {
                    Promotions.Add(new KhuyenMai
                    {
                        MaKhuyenMai = (int)row["MaKhuyenMai"],
                        TenChuongTrinh = row["TenChuongTrinh"].ToString(),
                        MaCode = row["MaCode"].ToString(),
                        LoaiGiamGia = row["LoaiGiamGia"].ToString(),
                        GiaTriGiam = (decimal)row["GiaTriGiam"],
                        NgayBatDau = (DateTime)row["NgayBatDau"],
                        NgayKetThuc = (DateTime)row["NgayKetThuc"],
                        TrangThai = row["TrangThai"].ToString(),
                        SoLuotSuDungToiDa = row["SoLuotSuDungToiDa"] == DBNull.Value ? null : (int?)row["SoLuotSuDungToiDa"],
                        SoLuotDaSuDung = (int)row["SoLuotDaSuDung"]
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message);
            }
        }

        private void AddPromotion()
        {
            var dialog = new AddPromotionWindow();
            if (dialog.ShowDialog() == true)
            {
                LoadPromotions();
            }
        }

        private void EditPromotion(KhuyenMai promotion)
        {
            if (promotion == null)
            {
                MessageBox.Show("Chọn khuyến mãi để sửa!");
                return;
            }

            var dialog = new AddPromotionWindow(promotion);
            if (dialog.ShowDialog() == true)
            {
                LoadPromotions();
            }
        }

        private void DeletePromotion(KhuyenMai promotion)
        {
            if (promotion == null)
            {
                MessageBox.Show("Chọn khuyến mãi để xóa!");
                return;
            }

            if (MessageBox.Show($"Xóa khuyến mãi '{promotion.MaCode}'?", "Xác nhận", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    // Xóa các món ăn áp dụng khuyến mãi trước
                    DatabaseConnect.ExecuteNonQuery(
                        "DELETE FROM MonAnApDungKM WHERE MaKhuyenMai = @id",
                        new SqlParameter("@id", promotion.MaKhuyenMai));

                    // Sau đó xóa khuyến mãi
                    DatabaseConnect.ExecuteNonQuery(
                        "DELETE FROM KhuyenMai WHERE MaKhuyenMai = @id",
                        new SqlParameter("@id", promotion.MaKhuyenMai));

                    MessageBox.Show("Xóa thành công!");
                    LoadPromotions();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xóa: " + ex.Message);
                }
            }
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;
        public void Execute(object parameter) => _execute(parameter);
    }
}