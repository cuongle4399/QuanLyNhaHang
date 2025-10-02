using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

namespace restaurantManager.Models
{
    public class BanAn : INotifyPropertyChanged
    {
        public string _maBan;

        public int _soGhe;

        public string _trangThai;

        private bool _isSelected;

        public string MaBan
        {
            get => _maBan;
            set
            {
                if (_maBan != value)
                {
                    _maBan = value;
                    OnPropertyChanged(nameof(MaBan));
                }
            }
        }

        public int SoGhe
        {
            get => _soGhe;
            set
            {
                if (_soGhe != value)
                {
                    _soGhe = value;
                    OnPropertyChanged(nameof(SoGhe));
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
                    OnPropertyChanged(nameof(TrangThai));
                }
            }
        }

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

        public BanAn (string maBan, int soGhe, string trangThai)
        {
            MaBan = maBan;
            SoGhe = soGhe;
            TrangThai = trangThai;
        }

        public BanAn() { }


        // Triển khai INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
