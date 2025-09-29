using System;
using System.ComponentModel;

namespace restaurantManager.Models
{
    public class ChiTiet : INotifyPropertyChanged
    {
        private decimal _gia;
        private int _maMonAn;
        private int _soLuong;
        private int? _maKhuyenMai;
        private decimal _tienGoc;
        private decimal _soTienGiam;
        private decimal _thanhToanCuoi;
        private string _maNhanVien;
        private string _tenMonAn;

        public decimal Gia
        {
            get => _gia;
            set
            {
                if (_gia != value)
                {
                    _gia = value;
                    OnPropertyChanged(nameof(Gia));
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

        public string MaNhanVien
        {
            get => _maNhanVien;
            set
            {
                if (_maNhanVien != value)
                {
                    _maNhanVien = value;
                    OnPropertyChanged(nameof(MaNhanVien));
                }
            }
        }

        public string TenMonAn
        {
            get => _tenMonAn;
            set
            {
                if (_tenMonAn != value)
                {
                    _tenMonAn = value;
                    OnPropertyChanged(nameof(TenMonAn));
                }
            }
        }

        // 🔹 Constructor mặc định
        public ChiTiet() { }

        // 🔹 Constructor đầy đủ tham số
        public ChiTiet(string tenMonAn, int soLuong, decimal tienGoc, decimal soTienGiam, decimal thanhToanCuoi, int maMonAn, decimal gia, int? maKhuyenMai)
        {
            Gia = gia;
            MaMonAn = maMonAn;
            SoLuong = soLuong;
            MaKhuyenMai = maKhuyenMai;
            TienGoc = tienGoc;
            SoTienGiam = soTienGiam;
            ThanhToanCuoi = thanhToanCuoi;
            TenMonAn = tenMonAn;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
