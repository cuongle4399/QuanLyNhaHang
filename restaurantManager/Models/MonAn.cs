using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace restaurantManager.Models
{
    public class MonAn : INotifyPropertyChanged
    {
        private int _maMonAn;
        private string _tenMonAn;
        private decimal _gia;
        private string _moTa;
        private string _hinhAnhURL;
        private int _loai;
        private int _soLuong = 1;

        // ✅ Constructor mặc định
        public MonAn()
        {
            _soLuong = 1;
        }

        // ✅ Constructor với 6 tham số (phù hợp với LoadDanhSachMonAn)
        public MonAn(int maMonAn, string tenMonAn, decimal gia, string moTa, string hinhAnhURL, int loai)
        {
            _maMonAn = maMonAn;
            _tenMonAn = tenMonAn;
            _gia = gia;
            _moTa = moTa;
            _hinhAnhURL = hinhAnhURL;
            _loai = loai;
            _soLuong = 1;
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

        public decimal Gia
        {
            get => _gia;
            set
            {
                if (_gia != value)
                {
                    _gia = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(TongTien));
                }
            }
        }

        public string MoTa
        {
            get => _moTa;
            set
            {
                if (_moTa != value)
                {
                    _moTa = value;
                    OnPropertyChanged();
                }
            }
        }

        public string HinhAnhURL
        {
            get => _hinhAnhURL;
            set
            {
                if (_hinhAnhURL != value)
                {
                    _hinhAnhURL = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Loai
        {
            get => _loai;
            set
            {
                if (_loai != value)
                {
                    _loai = value;
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
                    OnPropertyChanged(nameof(TongTien));
                }
            }
        }

        // Computed property
        public decimal TongTien
        {
            get => Gia * SoLuong;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}