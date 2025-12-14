using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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

        public ChiTietDonHang()
        {
        }

        public int MaDonHang
        {
            get => _maDonHang;
            set
            {
                if (_maDonHang != value)
                {
                    _maDonHang = value;
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}