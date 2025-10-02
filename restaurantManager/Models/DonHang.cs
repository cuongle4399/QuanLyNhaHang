using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace restaurantManager.Models
{
    public class DonHang : INotifyPropertyChanged
    {
        private int _maDonHang;
        private DateTime _ngayDat;
        private decimal _tongTien;
        private string _maNhanVien;
        private string _trangThai;
        private string _maBan;

        public int MaDonHang
        {
            get => _maDonHang;
            set => SetField(ref _maDonHang, value);
        }

        public DateTime NgayDat
        {
            get => _ngayDat;
            set => SetField(ref _ngayDat, value);
        }

        public decimal TongTien
        {
            get => _tongTien;
            set => SetField(ref _tongTien, value);
        }

        public string MaNhanVien
        {
            get => _maNhanVien;
            set => SetField(ref _maNhanVien, value);
        }

        public string TrangThai
        {
            get => _trangThai;
            set => SetField(ref _trangThai, value);
        }

        public string MaBan
        {
            get => _maBan;
            set => SetField(ref _maBan, value);
        }

        public DonHang (int maDonHang, DateTime ngayDat, decimal tongTien, string maNhanVien, string trangThai, string maBan)
        {
            MaDonHang = maDonHang;
            NgayDat = ngayDat;
            TongTien = tongTien;
            MaNhanVien = maNhanVien;
            TrangThai = trangThai;
            MaBan = maBan;
        }

        public DonHang() { }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}

