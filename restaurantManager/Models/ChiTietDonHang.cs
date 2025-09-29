using System;
using System.ComponentModel;

namespace restaurantManager.Models
{
    public class ChiTietDonHang : INotifyPropertyChanged
    {
        private int _maDonHang;
        private int _maMonAn;
        private int _soLuong;
        private int? _maKhuyenMai;
        private decimal _tienGoc;
        private decimal _soTienGiam;
        private decimal _thanhToanCuoi;

        public int MaDonHang
        {
            get => _maDonHang;
            set
            {
                if (_maDonHang != value)
                {
                    _maDonHang = value;
                    OnPropertyChanged(nameof(MaDonHang));
                }
            }
        }

        public int MaMonAn
        {
            get => _maMonAn;
            set
            {
                if (_maMonAn != value)
                {
                    _maMonAn = value;
                    OnPropertyChanged(nameof(MaMonAn));
                }
            }
        }

        public int SoLuong
        {
            get => _soLuong;
            set
            {
                if (_soLuong != value)
                {
                    _soLuong = value;
                    OnPropertyChanged(nameof(SoLuong));
                }
            }
        }

        public int? MaKhuyenMai
        {
            get => _maKhuyenMai;
            set
            {
                if (_maKhuyenMai != value)
                {
                    _maKhuyenMai = value;
                    OnPropertyChanged(nameof(MaKhuyenMai));
                }
            }
        }

        public decimal TienGoc
        {
            get => _tienGoc;
            set
            {
                if (_tienGoc != value)
                {
                    _tienGoc = value;
                    OnPropertyChanged(nameof(TienGoc));
                }
            }
        }

        public decimal SoTienGiam
        {
            get => _soTienGiam;
            set
            {
                if (_soTienGiam != value)
                {
                    _soTienGiam = value;
                    OnPropertyChanged(nameof(SoTienGiam));
                }
            }
        }

        public decimal ThanhToanCuoi
        {
            get => _thanhToanCuoi;
            set
            {
                if (_thanhToanCuoi != value)
                {
                    _thanhToanCuoi = value;
                    OnPropertyChanged(nameof(ThanhToanCuoi));
                }
            }
        }

        public ChiTietDonHang(int maDonHang, int maMonAn, int soLuong, int maKhuyenMai, decimal tienGoc, decimal soTienGiam, decimal thanhToanCuoi)
        {
            MaDonHang = maDonHang;
            MaMonAn = maMonAn;
            SoLuong = soLuong;
            MaKhuyenMai = maKhuyenMai;
            TienGoc = tienGoc;
            SoTienGiam = soTienGiam;
            ThanhToanCuoi = thanhToanCuoi;
        }

        public ChiTietDonHang() { }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
