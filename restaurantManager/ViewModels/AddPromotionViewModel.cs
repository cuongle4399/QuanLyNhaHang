using restaurantManager.Models;
using restaurantManager.Services;
using restaurantManager.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;

namespace restaurantManager.ViewModels
{
    public class AddPromotionViewModel : BaseViewModel
    {
        private const string ConnectionString = @"Data Source=CUONG-LE;Initial Catalog=QuanLyNhaHang;Integrated Security=True";
        private KhuyenMai _selectedPromotion;
        private string _tenChuongTrinh;
        private string _maCode;
        private string _loaiGiamGia;
        private decimal _giaTriGiam;
        private DateTime _ngayBatDau;
        private DateTime _ngayKetThuc;
        private int? _soLuotSuDungToiDa;

        public string TenChuongTrinh { get => _tenChuongTrinh; set { _tenChuongTrinh = value; OnPropertyChanged(); } }
        public string MaCode { get => _maCode; set { _maCode = value; OnPropertyChanged(); } }
        public string LoaiGiamGia { get => _loaiGiamGia; set { _loaiGiamGia = value; OnPropertyChanged(); } }
        public decimal GiaTriGiam { get => _giaTriGiam; set { _giaTriGiam = value; OnPropertyChanged(); } }
        public DateTime NgayBatDau { get => _ngayBatDau; set { _ngayBatDau = value; OnPropertyChanged(); } }
        public DateTime NgayKetThuc { get => _ngayKetThuc; set { _ngayKetThuc = value; OnPropertyChanged(); } }
        public int? SoLuotSuDungToiDa { get => _soLuotSuDungToiDa; set { _soLuotSuDungToiDa = value; OnPropertyChanged(); } }

        public ObservableCollection<MonAnItem> MonAnList { get; set; }
        public List<int> SelectedMonAnList { get; set; } = new List<int>();
        public bool IsEditMode { get; set; }

        public AddPromotionViewModel()
        {
            MonAnList = new ObservableCollection<MonAnItem>();
            NgayBatDau = DateTime.Now;
            NgayKetThuc = DateTime.Now.AddDays(7);
            LoaiGiamGia = "PhanTram";
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
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("SELECT MaMonAn FROM MonAnApDungKM WHERE MaKhuyenMai = @MaKhuyenMai", conn))
                    {
                        cmd.Parameters.AddWithValue("@MaKhuyenMai", maKhuyenMai);
                        using (var reader = cmd.ExecuteReader())
                            while (reader.Read())
                                list.Add(reader.GetInt32(0));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải món ăn: " + ex.Message);
            }
            return list;
        }

        private void LoadMonAnList()
        {
            MonAnList.Clear();
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("SELECT MaMonAn, TenMonAn FROM MonAn ORDER BY TenMonAn", conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int maMonAn = reader.GetInt32(0);
                                MonAnList.Add(new MonAnItem
                                {
                                    MaMonAn = maMonAn,
                                    TenMonAn = reader.GetString(1),
                                    IsSelected = SelectedMonAnList.Contains(maMonAn)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải món ăn: " + ex.Message);
            }
        }

        public void Save()
        {
            if (!ValidateInput()) return;

            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var trans = conn.BeginTransaction())
                    {
                        try
                        {
                            int maKhuyenMai = SavePromotion(conn, trans);
                            SaveMonAnApDung(conn, trans, maKhuyenMai);
                            trans.Commit();
                            MessageBox.Show(IsEditMode ? "Cập nhật thành công!" : "Thêm mới thành công!",
                                "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(TenChuongTrinh))
            {
                MessageBox.Show("Tên chương trình không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(MaCode))
            {
                MessageBox.Show("Mã giảm giá không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (GiaTriGiam <= 0)
            {
                MessageBox.Show("Giá trị giảm phải lớn hơn 0!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (NgayKetThuc <= NgayBatDau)
            {
                MessageBox.Show("Ngày kết thúc phải sau ngày bắt đầu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            // Kiểm tra LoaiGiamGia - phải là "PhanTram" hoặc "TienCoDinh"
            if (string.IsNullOrEmpty(LoaiGiamGia) ||
                (LoaiGiamGia != "PhanTram" && LoaiGiamGia != "TienCoDinh"))
            {
                MessageBox.Show("Vui lòng chọn loại giảm giá (Giảm % hoặc Giảm tiền)!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (SoLuotSuDungToiDa.HasValue && SoLuotSuDungToiDa <= 0)
            {
                MessageBox.Show("Số lượt sử dụng tối đa phải lớn hơn 0!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!IsEditMode && MaCodeExists(MaCode))
            {
                MessageBox.Show("Mã giảm giá đã tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (IsEditMode && _selectedPromotion.MaCode != MaCode && MaCodeExists(MaCode))
            {
                MessageBox.Show("Mã giảm giá đã tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private int SavePromotion(SqlConnection conn, SqlTransaction trans)
        {
            if (IsEditMode)
            {
                var cmd = new SqlCommand(@"UPDATE KhuyenMai SET 
                    TenChuongTrinh=@TenChuongTrinh, 
                    MaCode=@MaCode, 
                    LoaiGiamGia=@LoaiGiamGia, 
                    GiaTriGiam=@GiaTriGiam, 
                    NgayBatDau=@NgayBatDau, 
                    NgayKetThuc=@NgayKetThuc, 
                    SoLuotSuDungToiDa=@SoLuotSuDungToiDa 
                    WHERE MaKhuyenMai=@MaKhuyenMai", conn, trans);

                cmd.Parameters.AddWithValue("@TenChuongTrinh", TenChuongTrinh ?? "");
                cmd.Parameters.AddWithValue("@MaCode", MaCode ?? "");
                cmd.Parameters.AddWithValue("@LoaiGiamGia", LoaiGiamGia ?? "PhanTram");
                cmd.Parameters.AddWithValue("@GiaTriGiam", GiaTriGiam);
                cmd.Parameters.AddWithValue("@NgayBatDau", NgayBatDau);
                cmd.Parameters.AddWithValue("@NgayKetThuc", NgayKetThuc);
                cmd.Parameters.AddWithValue("@SoLuotSuDungToiDa", SoLuotSuDungToiDa ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaKhuyenMai", _selectedPromotion.MaKhuyenMai);
                cmd.ExecuteNonQuery();
                return _selectedPromotion.MaKhuyenMai;
            }
            else
            {
                var cmd = new SqlCommand(@"INSERT INTO KhuyenMai 
                    (TenChuongTrinh, MaCode, LoaiGiamGia, GiaTriGiam, 
                    NgayBatDau, NgayKetThuc, TrangThai, SoLuotSuDungToiDa, SoLuotDaSuDung) 
                    VALUES 
                    (@TenChuongTrinh, @MaCode, @LoaiGiamGia, @GiaTriGiam, 
                    @NgayBatDau, @NgayKetThuc, 'HoatDong', @SoLuotSuDungToiDa, 0);
                    SELECT SCOPE_IDENTITY();", conn, trans);

                cmd.Parameters.AddWithValue("@TenChuongTrinh", TenChuongTrinh ?? "");
                cmd.Parameters.AddWithValue("@MaCode", MaCode ?? "");
                cmd.Parameters.AddWithValue("@LoaiGiamGia", LoaiGiamGia ?? "PhanTram");
                cmd.Parameters.AddWithValue("@GiaTriGiam", GiaTriGiam);
                cmd.Parameters.AddWithValue("@NgayBatDau", NgayBatDau);
                cmd.Parameters.AddWithValue("@NgayKetThuc", NgayKetThuc);
                cmd.Parameters.AddWithValue("@SoLuotSuDungToiDa", SoLuotSuDungToiDa ?? (object)DBNull.Value);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        private void SaveMonAnApDung(SqlConnection conn, SqlTransaction trans, int maKhuyenMai)
        {
            new SqlCommand("DELETE FROM MonAnApDungKM WHERE MaKhuyenMai = @MaKhuyenMai", conn, trans)
            { Parameters = { new SqlParameter("@MaKhuyenMai", maKhuyenMai) } }.ExecuteNonQuery();

            foreach (var monAn in MonAnList.Where(m => m.IsSelected))
            {
                var cmd = new SqlCommand("INSERT INTO MonAnApDungKM (MaKhuyenMai, MaMonAn) VALUES (@MaKhuyenMai, @MaMonAn)", conn, trans);
                cmd.Parameters.AddWithValue("@MaKhuyenMai", maKhuyenMai);
                cmd.Parameters.AddWithValue("@MaMonAn", monAn.MaMonAn);
                cmd.ExecuteNonQuery();
            }
        }

        private bool MaCodeExists(string maCode)
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("SELECT COUNT(*) FROM KhuyenMai WHERE MaCode = @MaCode", conn))
                    {
                        cmd.Parameters.AddWithValue("@MaCode", maCode);
                        return (int)cmd.ExecuteScalar() > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }

    public class MonAnItem : BaseViewModel
    {
        private int _maMonAn;
        private string _tenMonAn;
        private bool _isSelected;

        public int MaMonAn { get => _maMonAn; set { _maMonAn = value; OnPropertyChanged(); } }
        public string TenMonAn { get => _tenMonAn; set { _tenMonAn = value; OnPropertyChanged(); } }
        public bool IsSelected { get => _isSelected; set { _isSelected = value; OnPropertyChanged(); } }
    }
}