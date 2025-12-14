using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace restaurantManager.Models
{
    public class DonHang : INotifyPropertyChanged
    {
        private int _maDonHang;
        private DateTime _ngayDat;
        private decimal _tongTien;
        private string _maNhanVien;
        private string _trangThai;
        private string _hinhThucThanhToan;
        private string _maBan;

        public DonHang()
        {
            _ngayDat = DateTime.Now;
            _hinhThucThanhToan = "TienMat";
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

        public DateTime NgayDat
        {
            get => _ngayDat;
            set
            {
                if (_ngayDat != value)
                {
                    _ngayDat = value;
                    OnPropertyChanged();
                }
            }
        }

        public decimal TongTien
        {
            get => _tongTien;
            set
            {
                if (_tongTien != value)
                {
                    _tongTien = value;
                    OnPropertyChanged();
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
                    OnPropertyChanged();
                }
            }
        }

        public string TrangThai
        {
            get => _trangThai;
            set
            {
                if (_trangThai != value)
                {
                    _trangThai = value;
                    OnPropertyChanged();
                }
            }
        }

        // ✅ PROPERTY CHÍNH: HinhThucThanhToan
        public string HinhThucThanhToan
        {
            get => _hinhThucThanhToan;
            set
            {
                if (_hinhThucThanhToan != value)
                {
                    _hinhThucThanhToan = value;
                    OnPropertyChanged();
                }
            }
        }

        public string MaBan
        {
            get => _maBan;
            set
            {
                if (_maBan != value)
                {
                    _maBan = value;
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