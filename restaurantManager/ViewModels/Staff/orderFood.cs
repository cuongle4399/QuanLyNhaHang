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

        public ICommand TangSoLuongCommand { get; }

        public ICommand GiamSoLuongCommand{ get; }

        public ICommand XacNhanDatBanCommand { get; }

        private readonly OrderService _orderService;

        public orderFood()
        {
            _orderService = new OrderService();

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

            TangSoLuongCommand = new RelayCommand<MonAn>(mon =>
            {
                if (mon == null) return;
                mon.SoLuong++;
                OnPropertyChanged(nameof(TongTienGioHang)); // cập nhật tổng tiền
            });

            GiamSoLuongCommand = new RelayCommand<MonAn>(mon =>
            {
                if (mon == null) return;

                if (mon.SoLuong > 1)
                {
                    mon.SoLuong--;
                }
                else
                {
                    // Nếu số lượng = 1 thì giảm nữa sẽ xóa khỏi giỏ
                    GioHang.Remove(mon);
                }

                OnPropertyChanged(nameof(TongTienGioHang));
            });

            XacNhanDatBanCommand = new RelayCommand(() => XacNhanDatBan("NV001"));


            DanhSachBanAn =  _orderService.LayDanhSachBanAnTuDb();
            DanhSachMonAn = _orderService.LayDanhSachMonAnTuBd();
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


        private string _thongBao;
        public string ThongBao
        {
            get => _thongBao;
            set
            {
                _thongBao = value;
                OnPropertyChanged();
            }
        }


        public void XacNhanDatBan(string maNV)
        {
            var selectedBan = DanhSachBanAn.FirstOrDefault(b => b.IsSelected);

            if (selectedBan == null || GioHang.Count == 0)
            {
                // Không gọi MessageBox ở đây
                ThongBao = "Chọn bàn và món trước khi xác nhận!";
                return;
            }

            if (selectedBan.TrangThai == "Đã đặt")
            {
                ThongBao = $"Bàn {selectedBan.MaBan} đã có khách, vui lòng chọn bàn khác!";
                return;
            }

            // Gọi service để update DB
            _orderService.CapNhatTrangThaiBan(selectedBan.MaBan, "Đã đặt");

            var dh = new DonHang
            {
                NgayDat = DateTime.Now,
                MaBan = selectedBan.MaBan,
                MaNhanVien = maNV,
                TrangThai = "DangXuLy",
                TongTien = 0
            };

            int maDH = _orderService.ChenDonHang(dh);

            MessageBox.Show("Mã đơn hàng: " + maDH);


            foreach (var mon in GioHang)
            {
                int maKM = _orderService.LayMaKhuyenMaiCuaMonAn(mon.MaMonAn);

                KhuyenMai khuyenMai = _orderService.LayKhuyenMaiTheoMaKhuyenMai(maKM);

                decimal soTienGiam = 0;

                if (khuyenMai.LoaiGiamGia == "PhanTram") // Giảm theo phần trăm
                {
                    soTienGiam = (Convert.ToDecimal(mon.Gia) * mon.SoLuong) * (khuyenMai.GiaTriGiam / 100);
                }
                else if (khuyenMai.LoaiGiamGia == "TienCoDinh")
                {
                    soTienGiam = khuyenMai.GiaTriGiam;
                }

                var ct = new ChiTietDonHang
                {
                    MaDonHang = maDH,
                    MaMonAn = mon.MaMonAn,
                    SoLuong = mon.SoLuong,
                    MaKhuyenMai = maKM,
                    TienGoc = Convert.ToDecimal(mon.Gia) * mon.SoLuong,
                    SoTienGiam = khuyenMai.GiaTriGiam,
                    ThanhToanCuoi = (Convert.ToDecimal(mon.Gia) * mon.SoLuong) - soTienGiam
                };

                if (!_orderService.ChenChiTietDonHang(ct))
                {
                    ThongBao = "Đặt hàng thất bại.";
                    return;
                }
            }

            selectedBan.TrangThai = "Đã đặt";
            GioHang.Clear();

            ThongBao = "Xác nhận đặt bàn thành công!";
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
