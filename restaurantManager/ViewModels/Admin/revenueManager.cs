using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using restaurantManager.Services;

namespace restaurantManager.ViewModels.Admin
{
    internal class revenueManager
    {
        //public DataTable GetInvoices(DateTime from, DateTime to)
        //{

        //}

        public DataTable GetAllInvoices()
        {
            string query = @"
        SELECT 
            d.MaDonHang,
            d.NgayDat,
            SUM(ISNULL(c.SoLuong,0) * ISNULL(c.GiaTaiThoiDiem,0)) AS TongCacMon,
            CASE 
                WHEN SUM(ISNULL(c.SoLuong,0) * ISNULL(c.GiaTaiThoiDiem,0)) - ISNULL(d.GiaTriKhuyenMai,0) < 0
                     THEN 0
                ELSE SUM(ISNULL(c.SoLuong,0) * ISNULL(c.GiaTaiThoiDiem,0)) - ISNULL(d.GiaTriKhuyenMai,0)
            END AS ThanhToanCuoi,
            d.TrangThai,
            k.TenChuongTrinh,
            ISNULL(d.GiaTriKhuyenMai,0) AS GiaTriKhuyenMai
        FROM DonHang d
        LEFT JOIN ChiTietDonHang c ON c.MaDonHang = d.MaDonHang
        LEFT JOIN KhuyenMai k      ON d.MaKhuyenMai = k.MaKhuyenMai
        GROUP BY d.MaDonHang, d.NgayDat, d.TrangThai, k.TenChuongTrinh, d.GiaTriKhuyenMai
        ORDER BY d.NgayDat DESC";
            return DatabaseConnect.ExecuteTable(query);
        }

        //public DataRow GetRevenueSummary(DateTime from, DateTime to)
        //{
        //}

        public DataRow GetRevenueSummaryAll()
        {
            string query = @"
        SELECT 
            ISNULL(SUM(x.ThanhToanCuoi),0) AS TongDoanhThu,
            COUNT(*) AS SoHoaDon
        FROM (
            SELECT d.MaDonHang,
                   CASE 
                     WHEN SUM(ISNULL(c.SoLuong,0) * ISNULL(c.GiaTaiThoiDiem,0)) - ISNULL(d.GiaTriKhuyenMai,0) < 0
                       THEN 0
                     ELSE SUM(ISNULL(c.SoLuong,0) * ISNULL(c.GiaTaiThoiDiem,0)) - ISNULL(d.GiaTriKhuyenMai,0)
                   END AS ThanhToanCuoi
            FROM DonHang d
            LEFT JOIN ChiTietDonHang c ON c.MaDonHang = d.MaDonHang
            GROUP BY d.MaDonHang, d.GiaTriKhuyenMai
        ) x";
            var dt = DatabaseConnect.ExecuteTable(query);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }


        public DataTable GetInvoiceDetails(int maDonHang)
        {
            string query = @"
        SELECT 
            m.TenMonAn AS TenMon,
            c.SoLuong,
            c.GiaTaiThoiDiem AS DonGia,
            (ISNULL(c.SoLuong,0) * ISNULL(c.GiaTaiThoiDiem,0)) AS ThanhTien
        FROM ChiTietDonHang c
        INNER JOIN MonAn m ON c.MaMonAn = m.MaMonAn
        WHERE c.MaDonHang = @MaDonHang
        ORDER BY m.TenMonAn";
            return restaurantManager.Services.DatabaseConnect.ExecuteTable(
                query, new System.Data.SqlClient.SqlParameter("@MaDonHang", maDonHang));
        }

    }
}
