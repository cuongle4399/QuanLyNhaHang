using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

using restaurantManager.Models;
using System.Data;
using restaurantManager.Services;
using System.Windows;
using System.Windows.Input;

using GalaSoft.MvvmLight.Command;

namespace restaurantManager.ViewModels.Staff
{
    public class orderFood : INotifyPropertyChanged
    {
        public ICommand ChonTatCaCommand { get; }
        public ICommand ChonThucAnCommand { get; }
        public ICommand ChonThucUongCommand { get; }

        public ICommand HuyDonHangCommand { get; }

        public ICommand ChonBanCommand { get; }

        public ICommand ThemMonCommand { get; }

        public orderFood()
        {
            LoaiDangChon = -1;
            ChonTatCaCommand = new RelayCommand(() => LoaiDangChon = -1);
            ChonThucAnCommand = new RelayCommand(() => LoaiDangChon = 0);
            ChonThucUongCommand = new RelayCommand(() => LoaiDangChon = 1);

            HuyDonHangCommand = new RelayCommand(() => GioHang.Clear());

            ChonBanCommand = new RelayCommand<BanAn>(ban =>
            {
                if (ban == null) return;

                // Đặt bàn được chọn
                BanDangChon = ban;

                // Reset IsSelected tất cả bàn
                foreach (var b in DanhSachBanAn)
                    b.IsSelected = false;

                // Đánh dấu bàn đang chọn
                ban.IsSelected = true;
            });

            ThemMonCommand = new RelayCommand<MonAn>(mon =>
            {
                if (mon == null) return;

                // Kiểm tra nếu món đã có trong giỏ thì tăng số lượng
                var monTrongGio = GioHang.FirstOrDefault(m => m.MaMonAn == mon.MaMonAn);
                if (monTrongGio != null)
                {
                    monTrongGio.SoLuong++;
                }
                else
                {
                    // Copy đối tượng để không ảnh hưởng đến DanhSachMonAn
                    GioHang.Add(new MonAn
                    {
                        MaMonAn = mon.MaMonAn,
                        TenMonAn = mon.TenMonAn,
                        Gia = mon.Gia,
                        MoTa = mon.MoTa,
                        HinhAnhURL = mon.HinhAnhURL,
                        Loai = mon.Loai,
                        SoLuong = 1
                    });
                }

                // Notify thay đổi tổng tiền
                OnPropertyChanged(nameof(TongTienGioHang));
            });

            LayDanhSachBanAnTuDb();
            LayDanhSachMonAnTuBd();
            LocDanhSachTheoLoai();
        }

        public float TongTienGioHang
        {
            get => GioHang.Sum(m => m.Gia * m.SoLuong);
        }

        private BanAn _banDangChon;

        public BanAn BanDangChon
        {
            get => _banDangChon;
            set
            {
                if (_banDangChon == value) return; // Chọn bàn đã chọn

                // Bỏ chọn bàn dã chọn
                if (_banDangChon != null)
                    _banDangChon.IsSelected = false;

                _banDangChon = value; // Cập nhật lại bàn mới chọn

                // Đặt trạng thái là đã chọn
                if (_banDangChon != null)
                    _banDangChon.IsSelected = true;

                OnPropertyChanged(); // nếu cần notify SelectedItem binding
            }
        }

        private ObservableCollection<MonAn> _gioHang = new ObservableCollection<MonAn>();

        public ObservableCollection<MonAn> GioHang
        {
            get => _gioHang;
            set
            {
                if (_gioHang != value)
                {
                    _gioHang = value;
                    OnPropertyChanged();
                }
            }
        }

        // Muốn load danh sách bàn ăn thì cần phải có danh sách bàn ăn
        private ObservableCollection<BanAn> _danhSachBanAn = new ObservableCollection<BanAn>();

        public ObservableCollection<BanAn> DanhSachBanAn
        {
            get => _danhSachBanAn;
            set
            {
                if (_danhSachBanAn != value)
                {
                    _danhSachBanAn = value;
                    OnPropertyChanged();
                }
            }
        }

        private ObservableCollection<MonAn> _danhSachHienThi;
        public ObservableCollection<MonAn> DanhSachHienThi
        {
            get => _danhSachHienThi;
            set {
                _danhSachHienThi = value;
                OnPropertyChanged();
            }
        }

        // Muốn load món ăn thì cần có danh sách bàn ăn
        private ObservableCollection<MonAn> _danhSachMonAn = new ObservableCollection<MonAn>();

        public ObservableCollection<MonAn> DanhSachMonAn
        {
            get => _danhSachMonAn;
            set
            {
                if (_danhSachMonAn != value)
                {
                    _danhSachMonAn = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _loaiDangChon;
        public int LoaiDangChon
        {
            get => _loaiDangChon;
            set
            {
                _loaiDangChon = value;
                OnPropertyChanged(nameof(LoaiDangChon));
                LocDanhSachTheoLoai(); // Khi thay đổi loại -> cập nhật danh sách hiển thị
            }
        }


        // Load danh sách bàn ăn từ database
        private void LayDanhSachBanAnTuDb()
        {
            string sql = "SELECT MaBan, SoGhe, TrangThai FROM BanAn";

            DataTable dt = DatabaseConnect.ExecuteTable(sql);

            foreach (DataRow dr in dt.Rows) {
                string maBan = dr["MaBan"].ToString().Trim();
                int soGhe = Convert.ToInt32(dr["SoGhe"]);
                string trangThai = dr["TrangThai"].ToString().Trim();

                DanhSachBanAn.Add(new BanAn
                {
                    MaBan = maBan,
                    SoGhe = soGhe,
                    TrangThai = trangThai
                });
            }
        }

        // Load danh sách món ăn từ database
        private void LayDanhSachMonAnTuBd()
        {
            string sql = @"SELECT 
                                MaMonAn, 
                                TenMonAn, 
                                Gia, 
                                MoTa,
                                HinhAnhURL, 
                                Loai
                            FROM MonAn";

            DataTable dt = DatabaseConnect.ExecuteTable(sql);

            foreach (DataRow dr in dt.Rows)
            {
                int maMonAn = Convert.ToInt32(dr["MaMonAn"]);
                string tenMonAn = dr["TenMonAn"].ToString().Trim();
                float gia = float.Parse(dr["Gia"].ToString().Trim());
                string moTa = dr["MoTa"].ToString().Trim();
                string hinhAnhURL = dr["HinhAnhURL"].ToString().Trim(); ;
                if (hinhAnhURL[0] != '/') hinhAnhURL = '/' + hinhAnhURL;
                int loai = Convert.ToInt32(dr["Loai"]);

                DanhSachMonAn.Add(new MonAn
                {
                    MaMonAn = maMonAn,
                    TenMonAn = tenMonAn,
                    Gia = gia,
                    MoTa = moTa,
                    HinhAnhURL = hinhAnhURL,
                    Loai = loai
                });
            }
        }

        private void LocDanhSachTheoLoai()
        {
            if (LoaiDangChon == -1)
            {
                DanhSachHienThi = DanhSachMonAn;
            } else
            {
                DanhSachHienThi = new ObservableCollection<MonAn>(
                    // Khi món ăn thuộc loại đang chọn thì thêm vào danh sách hiển thị
                    DanhSachMonAn.Where(m => m.Loai == LoaiDangChon)
                );
            }
        }



        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
