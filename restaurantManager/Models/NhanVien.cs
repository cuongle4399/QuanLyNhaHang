
﻿using System;
using System.ComponentModel;

namespace restaurantManager.Models
{
    public class NhanVien : INotifyPropertyChanged
    {
        private string _maNhanVien;
        private string _hoTen;
        private DateTime _ngaySinh;
        private string _soDienThoai;
        private string _diaChi;
        private decimal _luong;
        private string _chucVu;
        private string _trangThai;

        public string MaNhanVien
        {
            get => _maNhanVien;
            set { _maNhanVien = value; OnPropertyChanged(nameof(MaNhanVien)); }
        }

        public string HoTen
        {
            get => _hoTen;
            set { _hoTen = value; OnPropertyChanged(nameof(HoTen)); }
        }

        public DateTime NgaySinh
        {
            get => _ngaySinh;
            set { _ngaySinh = value; OnPropertyChanged(nameof(NgaySinh)); }
        }

        public string SoDienThoai
        {
            get => _soDienThoai;
            set { _soDienThoai = value; OnPropertyChanged(nameof(SoDienThoai)); }
        }

        public string DiaChi
        {
            get => _diaChi;
            set { _diaChi = value; OnPropertyChanged(nameof(DiaChi)); }
        }

        public decimal Luong
        {
            get => _luong;
            set { _luong = value; OnPropertyChanged(nameof(Luong)); }
        }

        public string ChucVu
        {
            get => _chucVu;
            set { _chucVu = value; OnPropertyChanged(nameof(ChucVu)); }
        }

        public string TrangThai
        {
            get => _trangThai;
            set { _trangThai = value; OnPropertyChanged(nameof(TrangThai)); }
        }

        public NhanVien() { }

        public NhanVien(string maNhanVien, string hoTen, DateTime ngaySinh, string soDienThoai,
                        string diaChi, decimal luong, string chucVu, string trangThai)
        {
            MaNhanVien = maNhanVien;
            HoTen = hoTen;
            NgaySinh = ngaySinh;
            SoDienThoai = soDienThoai;
            DiaChi = diaChi;
            Luong = luong;
            ChucVu = chucVu;
            TrangThai = trangThai;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
