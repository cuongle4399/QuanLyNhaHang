using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Input;
using System.Xml.Linq;
using restaurantManager.Models;
using restaurantManager.Services;

namespace restaurantManager.ViewModels.Admin
{
    public class revenueManager : INotifyPropertyChanged
    {
        private DataView _invoices;
        private string _totalRevenueText = "0 VNĐ"; //tổng doanh thu hiển thị
        private string _invoiceCountText = "0"; //số hóa đơn hiển thị

        public event PropertyChangedEventHandler PropertyChanged;

        //dataView để hiển thị danh sách hóa đơn
        public DataView Invoices
        {
            get => _invoices;
            private set { _invoices = value; OnPropertyChanged(nameof(Invoices)); }
        }

        //chuỗi hiển thị tổng doanh thu
        public string TotalRevenueText
        {
            get => _totalRevenueText;
            private set { _totalRevenueText = value; OnPropertyChanged(nameof(TotalRevenueText)); }
        }

        //chuỗi hiển thị số hóa đơn
        public string InvoiceCountText
        {
            get => _invoiceCountText;
            private set { _invoiceCountText = value; OnPropertyChanged(nameof(InvoiceCountText)); }
        }
        public (DataRow invoiceRow, DataTable details) GetInvoiceWithDetails(DataRowView drv)
        {
            if (drv == null) return (null, null);

            DataRow invoiceRow = drv.Row;
            string maDonHang = Convert.ToString(invoiceRow["MaDonHang"]);
            DataTable details = LoadInvoiceDetails(maDonHang);

            return (invoiceRow, details);
        }
        public (DataRow invoiceHeader, DataTable details) GetInvoiceDataForDetail(DataRowView rowView)
        {
            if (rowView == null) return (null, null);

            string ma = Convert.ToString(rowView.Row["MaDonHang"]);
            var header = GetInvoiceHeader(ma);
            var details = LoadInvoiceDetails(ma);
            return (header, details);
        }

        public revenueManager()
        {
        }

        //Load tất cả hóa đơn và tính tổng doanh thu
        public void LoadAll()
        {
            var dt = GetInvoiceListAll();
            Invoices = dt.DefaultView;
            ApplySummary(GetRevenueSummaryAll());
        }

        //
        public void Search(DateTime from, DateTime to)
        {
            var dt = GetInvoiceList(from, to);
            Invoices = dt.DefaultView;
            ApplySummary(GetRevenueSummary(from, to));
        }

        //áp dụng tóm tắt doanh thu và số hóa đơn từ DataRow
        private void ApplySummary(DataRow row)
        {
            decimal total = 0; int count = 0;
            if (row != null)
            {
                total = row.Field<decimal>("TongDoanhThu");
                count = Convert.ToInt32(row["SoHoaDon"]);
            }
            TotalRevenueText = string.Format(new CultureInfo("vi-VN"), "{0:N0} VNĐ", total);
            InvoiceCountText = count.ToString();
        }

        //lấy danh sách tất cả hóa đơn
        private DataTable GetInvoiceListAll()
        {
            string sql = @"SELECT MaDonHang, NgayDat, TrangThai, MaBan, TongTienGoc, GiaTriKhuyenMai AS KhuyenMai, ThanhToanCuoi AS TongTien
                           FROM DonHang ORDER BY NgayDat DESC";
            return DatabaseConnect.ExecuteTable(sql);
        }

        //lấy danh sách hóa đơn trong khoảng ngày
        private DataTable GetInvoiceList(DateTime from, DateTime to)
        {
            string sql = @"SELECT MaDonHang, NgayDat, TrangThai, MaBan, ThanhToanCuoi AS TongTien
                           FROM DonHang
                           WHERE CONVERT(date, NgayDat) BETWEEN @from AND @to
                           ORDER BY NgayDat DESC";
            return DatabaseConnect.ExecuteTable(sql,
                new SqlParameter("@from", from.Date),
                new SqlParameter("@to", to.Date));
        }



        //lấy tổng doanh thu và số hóa đơn cho tất cả thời gian
        private DataRow GetRevenueSummaryAll()
        {
            string sql = @"SELECT ISNULL(SUM(ThanhToanCuoi),0) AS TongDoanhThu,
                                  COUNT(*) AS SoHoaDon
                           FROM DonHang";
            var dt = DatabaseConnect.ExecuteTable(sql);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }


        //lấy tất cả doanh thu và số hóa đơn trong khoảng ngày
        private DataRow GetRevenueSummary(DateTime from, DateTime to)
        {
            string sql = @"SELECT ISNULL(SUM(ThanhToanCuoi),0) AS TongDoanhThu,
                                  COUNT(*) AS SoHoaDon
                           FROM DonHang
                           WHERE CONVERT(date, NgayDat) BETWEEN @from AND @to";
            var dt = DatabaseConnect.ExecuteTable(sql,
                new SqlParameter("@from", from.Date),
                new SqlParameter("@to", to.Date));
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }
        private DataRow GetInvoiceHeader(string maDonHang)
        {
            string sql = @"
        SELECT  d.MaDonHang,
                d.NgayDat,
                d.TrangThai,
                d.TongTienGoc,
                d.GiaTriKhuyenMai,
                d.ThanhToanCuoi,
                km.TenChuongTrinh
        FROM DonHang d
        LEFT JOIN KhuyenMai km ON d.MaKhuyenMai = km.MaKhuyenMai
        WHERE d.MaDonHang = @Ma";

            var dt = DatabaseConnect.ExecuteTable(sql, new SqlParameter("@Ma", maDonHang));
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }

        private DataTable LoadInvoiceDetails(string maDonHang)
        {
            string sql = @"
        SELECT 
            MA.TenMonAn AS TenMon,
            CT.SoLuong,
            CT.GiaTaiThoiDiem AS DonGia,
            (CT.SoLuong * CT.GiaTaiThoiDiem) AS ThanhTien
        FROM ChiTietDonHang CT
        JOIN MonAn MA ON MA.MaMonAn = CT.MaMonAn
        WHERE CT.MaDonHang = @MaDonHang
        ORDER BY MA.TenMonAn;";

            return DatabaseConnect.ExecuteTable(sql, new SqlParameter("@MaDonHang", maDonHang));
        }

        //thông báo thay đổi thuộc tính
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}