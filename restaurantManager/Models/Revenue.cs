using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace restaurantManager.Models
{
    public class Order
    {
        public int MaDonHang { get; set; }
        public DateTime NgayDat { get; set; }
        public string TrangThai { get; set; }
        public string MaBan { get; set; }
        public decimal ThanhToanCuoi { get; set; }
    }
}
