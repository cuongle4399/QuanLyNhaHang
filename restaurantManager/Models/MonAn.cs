using System;
using System.ComponentModel;

namespace restaurantManager.Models
{
    public class MonAn : INotifyPropertyChanged
    {
        private int _maMonAn;
        private string _tenMonAn;
        private float _gia;
        private string _moTa;
        private string _hinhAnhURL;
        private int _loai;
        private int _soLuong;
        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
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

        public float Gia
        {
            get => _gia;
            set
            {
                if (_gia != value)
                {
                    _gia = value;
                    OnPropertyChanged(nameof(Gia));
                    OnPropertyChanged(nameof(TongTien)); // cập nhật tổng tiền khi giá thay đổi
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
                    OnPropertyChanged(nameof(MoTa));
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
                    OnPropertyChanged(nameof(HinhAnhURL));
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
                    OnPropertyChanged(nameof(Loai));
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
                    OnPropertyChanged(nameof(TongTien)); // cập nhật tổng tiền khi số lượng thay đổi
                }
            }
        }

        // Property tính tổng tiền cho từng món
        public float TongTien => SoLuong * Gia;

        public MonAn(int maMonAn, string tenMonAn, float gia, string moTa, string hinhAnhURL, int loai)
        {
            MaMonAn = maMonAn;
            TenMonAn = tenMonAn;
            Gia = gia;
            MoTa = moTa;
            HinhAnhURL = hinhAnhURL;
            Loai = loai;
            SoLuong = 1;
        }

        public MonAn()
        {
            SoLuong = 1;
        }

        // Triển khai INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
