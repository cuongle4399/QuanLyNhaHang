using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using restaurantManager.Models;
using restaurantManager.ViewModel;

namespace restaurantManager.ViewModels
{
    public class AddPromotionViewModel : BaseViewModel
    {
        private readonly string _connectionString = "Data Source=DESKTOP-VQ8CCK0\\NHATNGUYEN;Initial Catalog=QuanLyNhaHang;Integrated Security=True";
        private KhuyenMai _selectedPromotion;

        public string TenChuongTrinh { get; set; }
        public string MaCode { get; set; }
        public string LoaiGiamGia { get; set; }
        public decimal GiaTriGiam { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime NgayKetThuc { get; set; }
        public int? SoLuotSuDungToiDa { get; set; }

        public ObservableCollection<MonAnItem> MonAnList { get; set; }
        public List<int> SelectedMonAnList { get; set; } = new List<int>();

        public bool IsEditMode { get; set; }

        public AddPromotionViewModel()
        {
            MonAnList = new ObservableCollection<MonAnItem>();
            LoadMonAnList();
        }

        public AddPromotionViewModel(KhuyenMai promotion) : this()
        {
            IsEditMode = true;
            _selectedPromotion = promotion;

            TenChuongTrinh = promotion.TenChuongTrinh;
            MaCode = promotion.MaCode;
            LoaiGiamGia = promotion.LoaiGiamGia;
            GiaTriGiam = promotion.GiaTriGiam;
            NgayBatDau = promotion.NgayBatDau;
            NgayKetThuc = promotion.NgayKetThuc;
            SoLuotSuDungToiDa = promotion.SoLuotSuDungToiDa;

            SelectedMonAnList = GetAppliedMonAnIds(promotion.MaKhuyenMai);
            LoadMonAnList();
        }

        private List<int> GetAppliedMonAnIds(int maKhuyenMai)
        {
            var list = new List<int>();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT MaMonAn FROM MonAnApDungKM WHERE MaKhuyenMai = @MaKhuyenMai";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaKhuyenMai", maKhuyenMai);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            list.Add(reader.GetInt32("MaMonAn"));
                    }
                }
            }
            return list;
        }

        private void LoadMonAnList()
        {
            MonAnList.Clear();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT MaMonAn, TenMonAn FROM MonAn ORDER BY TenMonAn";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MonAnList.Add(new MonAnItem
                            {
                                MaMonAn = reader.GetInt32("MaMonAn"),
                                TenMonAn = reader.GetString("TenMonAn"),
                                IsSelected = SelectedMonAnList.Contains(reader.GetInt32("MaMonAn"))
                            });
                        }
                    }
                }
            }
        }

        public void Save()
        {
            if (string.IsNullOrWhiteSpace(TenChuongTrinh) || string.IsNullOrWhiteSpace(MaCode))
            {
                MessageBox.Show("Tên và mã giảm giá không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (GiaTriGiam <= 0)
            {
                MessageBox.Show("Giá trị giảm phải lớn hơn 0!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (NgayKetThuc <= NgayBatDau)
            {
                MessageBox.Show("Ngày kết thúc phải sau ngày bắt đầu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (LoaiGiamGia != "PhanTram" && LoaiGiamGia != "TienCoDinh")
            {
                MessageBox.Show("Loại giảm giá không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (IsEditMode && !string.IsNullOrEmpty(_selectedPromotion.MaCode) && _selectedPromotion.MaCode != MaCode)
            {
                if (MaCodeExists(MaCode))
                {
                    MessageBox.Show("Mã giảm giá đã tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else if (!IsEditMode && MaCodeExists(MaCode))
            {
                MessageBox.Show("Mã giảm giá đã tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    using (var trans = conn.BeginTransaction())
                    {
                        int maKhuyenMai = 0;

                        if (IsEditMode)
                        {
                            string update = @"
                                UPDATE KhuyenMai SET 
                                    TenChuongTrinh = @TenChuongTrinh, MaCode = @MaCode, LoaiGiamGia = @LoaiGiamGia, 
                                    GiaTriGiam = @GiaTriGiam, NgayBatDau = @NgayBatDau, NgayKetThuc = @NgayKetThuc, 
                                    SoLuotSuDungToiDa = @SoLuotSuDungToiDa
                                WHERE MaKhuyenMai = @MaKhuyenMai";
                            var cmd = new SqlCommand(update, conn, trans);
                            cmd.Parameters.AddWithValue("@TenChuongTrinh", TenChuongTrinh);
                            cmd.Parameters.AddWithValue("@MaCode", MaCode);
                            cmd.Parameters.AddWithValue("@LoaiGiamGia", LoaiGiamGia);
                            cmd.Parameters.AddWithValue("@GiaTriGiam", GiaTriGiam);
                            cmd.Parameters.AddWithValue("@NgayBatDau", NgayBatDau);
                            cmd.Parameters.AddWithValue("@NgayKetThuc", NgayKetThuc);
                            cmd.Parameters.AddWithValue("@SoLuotSuDungToiDa", SoLuotSuDungToiDa ?? (object)DBNull.Value);
                            cmd.Parameters.AddWithValue("@MaKhuyenMai", _selectedPromotion.MaKhuyenMai);
                            cmd.ExecuteNonQuery();
                            maKhuyenMai = _selectedPromotion.MaKhuyenMai;
                        }
                        else
                        {
                            string insert = @"
                                INSERT INTO KhuyenMai (TenChuongTrinh, MaCode, LoaiGiamGia, GiaTriGiam, NgayBatDau, NgayKetThuc, TrangThai, SoLuotSuDungToiDa, SoLuotDaSuDung)
                                VALUES (@TenChuongTrinh, @MaCode, @LoaiGiamGia, @GiaTriGiam, @NgayBatDau, @NgayKetThuc, 'HoatDong', @SoLuotSuDungToiDa, 0)";
                            var cmd = new SqlCommand(insert, conn, trans);
                            cmd.Parameters.AddWithValue("@TenChuongTrinh", TenChuongTrinh);
                            cmd.Parameters.AddWithValue("@MaCode", MaCode);
                            cmd.Parameters.AddWithValue("@LoaiGiamGia", LoaiGiamGia);
                            cmd.Parameters.AddWithValue("@GiaTriGiam", GiaTriGiam);
                            cmd.Parameters.AddWithValue("@NgayBatDau", NgayBatDau);
                            cmd.Parameters.AddWithValue("@NgayKetThuc", NgayKetThuc);
                            cmd.Parameters.AddWithValue("@SoLuotSuDungToiDa", SoLuotSuDungToiDa ?? (object)DBNull.Value);
                            cmd.ExecuteNonQuery();
                            maKhuyenMai = (int)cmd.ExecuteScalar();
                        }

                        // Xóa liên kết cũ
                        string deleteDetail = "DELETE FROM MonAnApDungKM WHERE MaKhuyenMai = @MaKhuyenMai";
                        var cmdDelete = new SqlCommand(deleteDetail, conn, trans);
                        cmdDelete.Parameters.AddWithValue("@MaKhuyenMai", maKhuyenMai);
                        cmdDelete.ExecuteNonQuery();

                        // Thêm mới
                        foreach (var item in MonAnList.Where(m => m.IsSelected))
                        {
                            string insertDetail = "INSERT INTO MonAnApDungKM (MaKhuyenMai, MaMonAn) VALUES (@MaKhuyenMai, @MaMonAn)";
                            var cmdInsert = new SqlCommand(insertDetail, conn, trans);
                            cmdInsert.Parameters.AddWithValue("@MaKhuyenMai", maKhuyenMai);
                            cmdInsert.Parameters.AddWithValue("@MaMonAn", item.MaMonAn);
                            cmdInsert.ExecuteNonQuery();
                        }

                        trans.Commit();
                        MessageBox.Show(IsEditMode ? "Cập nhật thành công!" : "Thêm mới thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool MaCodeExists(string maCode)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM KhuyenMai WHERE MaCode = @MaCode";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@MaCode", maCode);
                    return (int)cmd.ExecuteScalar() > 0;
                }
            }
        }

        public void Cancel() => Application.Current.MainWindow.Close();
    }

    public class MonAnItem
    {
        public int MaMonAn { get; set; }
        public string TenMonAn { get; set; }
        public bool IsSelected { get; set; }
    }
}