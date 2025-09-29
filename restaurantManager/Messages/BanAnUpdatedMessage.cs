using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace restaurantManager.Messages
{
    public class BanAnUpdatedMessage
    {
        public string MaBan { get; }
        public string TrangThai { get; }

        public BanAnUpdatedMessage(string maBan, string trangThai)
        {
            MaBan = maBan;
            TrangThai = trangThai;
        }
    }
}
