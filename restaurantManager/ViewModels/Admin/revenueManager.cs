using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using restaurantManager.Services;

namespace restaurantManager.ViewModels.Admin
{
    public class revenueManager : INotifyPropertyChanged
    {
        private DataView _invoices;
        private string _totalRevenueText;
        private string _invoiceCountText;

        public event PropertyChangedEventHandler PropertyChanged;

        public DataView Invoices
        {
            get => _invoices;
            set
            {
                if (_invoices != value)
                {
                    _invoices = value;
                    OnPropertyChanged();
                }
            }
        }

        public string TotalRevenueText
        {
            get => _totalRevenueText;
            set
            {
                if (_totalRevenueText != value)
                {
                    _totalRevenueText = value;
                    OnPropertyChanged();
                }
            }
        }

        public string InvoiceCountText
        {
            get => _invoiceCountText;
            set
            {
                if (_invoiceCountText != value)
                {
                    _invoiceCountText = value;
                    OnPropertyChanged();
                }
            }
        }

        public revenueManager()
        {
            LoadAll();
        }

        /// <summary>
        /// Tải tất cả hóa đơn
        /// </summary>
        public void LoadAll()
        {
            string query = @"
                SELECT 
                    d.MaDonHang,
                    d.NgayDat,
                    d.TongTien,
                    nv.HoTen AS TenNhanVien,
                    d.MaBan,
                    d.HinhThucThanhToan,
                    d.TrangThai
                FROM DonHang d
                INNER JOIN NhanVien nv ON d.MaNhanVien = nv.MaNhanVien
                WHERE d.TrangThai = 'DaHoanThanh'
                ORDER BY d.NgayDat DESC";

            DataTable dt = DatabaseConnect.ExecuteTable(query);
            Invoices = dt.DefaultView;
            UpdateStatistics();
        }

        /// <summary>
        /// Tìm kiếm doanh thu theo khoảng ngày
        /// </summary>
        public void Search(DateTime fromDate, DateTime toDate)
        {
            // Đảm bảo toDate bao gồm cả ngày cuối cùng (23:59:59)
            DateTime adjustedToDate = toDate.AddDays(1).AddSeconds(-1);

            string query = @"
                SELECT 
                    d.MaDonHang,
                    d.NgayDat,
                    d.TongTien,
                    nv.HoTen AS TenNhanVien,
                    d.MaBan,
                    d.HinhThucThanhToan,
                    d.TrangThai
                FROM DonHang d
                INNER JOIN NhanVien nv ON d.MaNhanVien = nv.MaNhanVien
                WHERE d.TrangThai = 'DaHoanThanh'
                  AND d.NgayDat >= @FromDate
                  AND d.NgayDat <= @ToDate
                ORDER BY d.NgayDat DESC";

            SqlParameter[] parameters = new[]
            {
                new SqlParameter("@FromDate", fromDate),
                new SqlParameter("@ToDate", adjustedToDate)
            };

            DataTable dt = DatabaseConnect.ExecuteTable(query, parameters);
            Invoices = dt.DefaultView;
            UpdateStatistics();
        }

        /// <summary>
        /// Cập nhật thống kê tổng doanh thu và số hóa đơn
        /// </summary>
        private void UpdateStatistics()
        {
            if (Invoices == null || Invoices.Count == 0)
            {
                TotalRevenueText = "0 VNĐ";
                InvoiceCountText = "0";
                return;
            }

            decimal totalRevenue = 0;
            int invoiceCount = Invoices.Count;

            foreach (DataRowView row in Invoices)
            {
                if (row["TongTien"] != DBNull.Value)
                {
                    totalRevenue += Convert.ToDecimal(row["TongTien"]);
                }
            }

            TotalRevenueText = totalRevenue.ToString("N0", new System.Globalization.CultureInfo("vi-VN")) + " VNĐ";
            InvoiceCountText = invoiceCount.ToString();
        }

        /// <summary>
        /// Lấy dữ liệu chi tiết hóa đơn
        /// </summary>
        public (DataRow invoiceRow, DataTable details) GetInvoiceDataForDetail(DataRowView drv)
        {
            if (drv == null)
                return (null, null);

            int maDonHang = Convert.ToInt32(drv["MaDonHang"]);

            // Lấy thông tin hóa đơn
            string queryInvoice = @"
                SELECT 
                    d.MaDonHang,
                    d.NgayDat,
                    nv.HoTen AS TenNhanVien,
                    d.MaBan,
                    d.HinhThucThanhToan,
                    d.TongTien AS ThanhToanCuoi,
                    ISNULL(SUM(ctd.SoTienGiam), 0) AS GiaTriKhuyenMai,
                    d.TongTien + ISNULL(SUM(ctd.SoTienGiam), 0) AS TongTienGoc
                FROM DonHang d
                INNER JOIN NhanVien nv ON d.MaNhanVien = nv.MaNhanVien
                LEFT JOIN ChiTietDonHang ctd ON d.MaDonHang = ctd.MaDonHang
                WHERE d.MaDonHang = @MaDonHang
                GROUP BY d.MaDonHang, d.NgayDat, nv.HoTen, d.MaBan, d.HinhThucThanhToan, d.TongTien";

            DataTable dtInvoice = DatabaseConnect.ExecuteTable(queryInvoice,
                new SqlParameter("@MaDonHang", maDonHang));

            if (dtInvoice == null || dtInvoice.Rows.Count == 0)
                return (null, null);

            DataRow invoiceRow = dtInvoice.Rows[0];

            // Lấy chi tiết các món ăn trong hóa đơn
            string queryDetails = @"
                SELECT 
                    ma.TenMonAn,
                    ctd.SoLuong,
                    ma.Gia,
                    ctd.TienGoc,
                    ctd.SoTienGiam,
                    ctd.ThanhToanCuoi,
                    ISNULL(km.MaCode, 'Không') AS MaKhuyenMai
                FROM ChiTietDonHang ctd
                INNER JOIN MonAn ma ON ctd.MaMonAn = ma.MaMonAn
                LEFT JOIN KhuyenMai km ON ctd.MaKhuyenMai = km.MaKhuyenMai
                WHERE ctd.MaDonHang = @MaDonHang
                ORDER BY ma.TenMonAn";

            DataTable dtDetails = DatabaseConnect.ExecuteTable(queryDetails,
                new SqlParameter("@MaDonHang", maDonHang));

            return (invoiceRow, dtDetails);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}