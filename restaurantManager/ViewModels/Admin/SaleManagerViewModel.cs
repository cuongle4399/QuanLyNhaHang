using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using restaurantManager.Models;
using restaurantManager.ViewModel;
using static restaurantManager.Models.KhuyenMai;

namespace  restaurantManager.ViewModels.Admin
{
    public class SaleManagerViewModel : BaseViewModel
    {
        public ObservableCollection<KhuyenMai> Promotions { get; set; }
        public PromotionStats Stats { get; set; }

        public string SearchText { get; set; }

        public ICommand LoadCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

        private readonly string _connectionString = "Data Source=DESKTOP-VQ8CCK0\\NHATNGUYEN;Initial Catalog=QuanLyNhaHang;Integrated Security=True";

        public SaleManagerViewModel()
        {
            Promotions = new ObservableCollection<KhuyenMai>();
            Stats = new PromotionStats();
            LoadCommand = new RelayCommand(LoadData);
            SearchCommand = new RelayCommand(SearchPromotions);
            AddCommand = new RelayCommand(AddPromotion);
            EditCommand = new RelayCommand(EditPromotion);
            DeleteCommand = new RelayCommand(DeletePromotion);

            LoadData();
        }

        private void LoadData()
        {
            Promotions.Clear();
            var promotions = GetPromotionsFromDatabase();
            foreach (var p in promotions)
            {
                Promotions.Add(p);
            }
            UpdateStats();
        }

        private List<KhuyenMai> GetPromotionsFromDatabase()
        {
            var result = new List<KhuyenMai>();

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
                    SELECT 
                        km.MaKhuyenMai,
                        km.TenChuongTrinh,
                        km.MaCode,
                        km.LoaiGiamGia,
                        km.GiaTriGiam,
                        km.NgayBatDau,
                        km.NgayKetThuc,
                        km.TrangThai,
                        km.SoLuotSuDungToiDa,
                        km.SoLuotDaSuDung,
                        STRING_AGG(ma.TenMonAn, ', ') AS MonAn
                    FROM KhuyenMai km
                    LEFT JOIN MonAnApDungKM kam ON km.MaKhuyenMai = kam.MaKhuyenMai
                    LEFT JOIN MonAn ma ON kam.MaMonAn = ma.MaMonAn
                    GROUP BY km.MaKhuyenMai, km.TenChuongTrinh, km.MaCode, km.LoaiGiamGia, 
                             km.GiaTriGiam, km.NgayBatDau, km.NgayKetThuc, km.TrangThai,
                             km.SoLuotSuDungToiDa, km.SoLuotDaSuDung
                    ORDER BY km.MaKhuyenMai";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var promo = new KhuyenMai
                            {
                                MaKhuyenMai = reader.GetInt32("MaKhuyenMai"),
                                TenChuongTrinh = reader.GetString("TenChuongTrinh"),
                                MaCode = reader.GetString("MaCode"),
                                LoaiGiamGia = reader.GetString("LoaiGiamGia"),
                                GiaTriGiam = reader.GetDecimal("GiaTriGiam"),
                                NgayBatDau = reader.GetDateTime("NgayBatDau"),
                                NgayKetThuc = reader.GetDateTime("NgayKetThuc"),
                                TrangThai = reader.GetString("TrangThai"),
                                SoLuotSuDungToiDa = reader.IsDBNull("SoLuotSuDungToiDa") ? null : reader.GetInt32("SoLuotSuDungToiDa"),
                                SoLuotDaSuDung = reader.GetInt32("SoLuotDaSuDung")
                            };
                            result.Add(promo);
                        }
                    }
                }
            }

            return result;
        }

        private void UpdateStats()
        {
            var total = Promotions.Count;
            var active = Promotions.Count(p => p.TrangThai == "HoatDong");
            var expired = Promotions.Count(p => p.TrangThai == "HetHan");

            decimal totalSaved = 0;
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT SUM(GiaTriKhuyenMai) FROM DonHang WHERE GiaTriKhuyenMai IS NOT NULL";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    var obj = cmd.ExecuteScalar();
                    if (obj != null && obj != DBNull.Value)
                        totalSaved = Convert.ToDecimal(obj);
                }
            }

            Stats.Total = total;
            Stats.Active = active;
            Stats.Expired = expired;
            Stats.TotalSaved = totalSaved;
        }

        private void SearchPromotions()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadData();
                return;
            }

            var filtered = GetPromotionsFromDatabase()
                .Where(p => p.MaCode.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                           p.TenChuongTrinh.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToList();

            Promotions.Clear();
            foreach (var p in filtered)
                Promotions.Add(p);
        }

        private KhuyenMai _selectedPromotion;
        public KhuyenMai SelectedPromotion
        {
            get => _selectedPromotion;
            set
            {
                _selectedPromotion = value;
                OnPropertyChanged();
            }
        }
        private void AddPromotion()
        {
            var window = new View.AddPromotionWindow();
            window.ShowDialog();
            if (window.DialogResult == true)
                LoadData();
        }

        private void EditPromotion()
        {
            if (SelectedPromotion == null)
            {
                MessageBox.Show("Vui lòng chọn một khuyến mãi để sửa.");
                return;
            }

            var window = new View.AddPromotionWindow(SelectedPromotion);
            window.ShowDialog();
            if (window.DialogResult == true)
                LoadData();
        }

        private void DeletePromotion()
        {
            if (SelectedPromotion == null)
            {
                MessageBox.Show("Vui lòng chọn một khuyến mãi để xóa.");
                return;
            }

            var result = MessageBox.Show($"Bạn có chắc chắn xóa khuyến mãi '{SelectedPromotion.MaCode}'?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No) return;

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        string deleteDetail = "DELETE FROM MonAnApDungKM WHERE MaKhuyenMai = @MaKhuyenMai";
                        var cmdDetail = new SqlCommand(deleteDetail, conn, trans);
                        cmdDetail.Parameters.AddWithValue("@MaKhuyenMai", SelectedPromotion.MaKhuyenMai);
                        cmdDetail.ExecuteNonQuery();

                        string deleteMain = "DELETE FROM KhuyenMai WHERE MaKhuyenMai = @MaKhuyenMai";
                        var cmdMain = new SqlCommand(deleteMain, conn, trans);
                        cmdMain.Parameters.AddWithValue("@MaKhuyenMai", SelectedPromotion.MaKhuyenMai);
                        cmdMain.ExecuteNonQuery();

                        trans.Commit();
                        LoadData();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        MessageBox.Show("Không thể xóa: " + ex.Message);
                    }
                }
            }
        }
    }
}