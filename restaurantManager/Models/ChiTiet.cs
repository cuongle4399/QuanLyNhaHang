using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace restaurantManager.Models
{
    public class ChiTiet : INotifyPropertyChanged
    {
        private int _maDonHang;
        private int _maMonAn;
        private string _tenMonAn;
        private int _soLuong;
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

        // ✅ FIX: Thêm property TenMonAn
        public string TenMonAn
        {
            get => _tenMonAn;
            set
            {
                if (_tenMonAn != value)
                {
                    _tenMonAn = value;
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