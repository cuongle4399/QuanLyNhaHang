using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace restaurantManager.Models
{
    public class BanAn : INotifyPropertyChanged
    {
        private string _maBan;
        private int _soGhe;
        private string _trangThai;
        private bool _isSelected;

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

        public int SoGhe
        {
            get => _soGhe;
            set
            {
                if (_soGhe != value)
                {
                    _soGhe = value;
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

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
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