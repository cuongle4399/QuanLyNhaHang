using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace restaurantManager.Models
{
    public class MonAn
    {
        public int MaMonAn { get; set; }
        public string TenMonAn { get; set; }
        public float Gia { get; set; }
        public string MoTa { get; set; }
        public string HinhAnhURL { get; set; }
        public int Loai { get; set; }


        public MonAn(int maMonAn, string tenMonAn, float gia, string moTa, string hinhAnhURL, int loai)
        {
            MaMonAn = maMonAn;
            TenMonAn = tenMonAn;
            Gia = gia;
            MoTa = moTa;
            HinhAnhURL = hinhAnhURL;
            Loai = loai;
        }
    }

}
