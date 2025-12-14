using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using restaurantManager.Models;
using restaurantManager.Services;
using restaurantManager.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

namespace restaurantManager.ViewModels.Staff
{
    public class orderFood : INotifyPropertyChanged
    {
        private readonly OrderService _orderService;

        public ObservableCollection<BanAn> DanhSachBanAn { get; set; }
        public ObservableCollection<MonAn> DanhSachMonAn { get; set; }

        private ObservableCollection<MonAn> _danhSachHienThi;
        public ObservableCollection<MonAn> DanhSachHienThi
        {
            get => _danhSachHienThi;
            set { _danhSachHienThi = value; OnPropertyChanged(); }
        }

        public ObservableCollection<MonAn> GioHang { get; set; } = new ObservableCollection<MonAn>();

        public decimal TongTienGioHang => GioHang.Sum(m => Convert.ToDecimal(m.Gia) * m.SoLuong);

        private string _tuKhoaTimKiem;
        public string TuKhoaTimKiem
        {
            get => _tuKhoaTimKiem;
            set { _tuKhoaTimKiem = value; OnPropertyChanged(); }
        }

        private BanAn _banDangChon;
        public BanAn BanDangChon
        {
            get => _banDangChon;
            set
            {
                if (_banDangChon == value) return;

                if (_banDangChon != null) _banDangChon.IsSelected = false;
                _banDangChon = value;
                if (_banDangChon != null) _banDangChon.IsSelected = true;

                OnPropertyChanged();
            }
        }

        public ObservableCollection<int> DanhSachMonAnCoKM { get; set; }

        private string _thongBao;
        public string ThongBao
        {
            get => _thongBao;
            set { _thongBao = value; OnPropertyChanged(); }
        }

        public ICommand ChonBanCommand { get; }
        public ICommand ThemMonCommand { get; }
        public ICommand TangSoLuongCommand { get; }
        public ICommand GiamSoLuongCommand { get; }
        public ICommand XacNhanDatBanCommand { get; }
        public ICommand HuyDonHangCommand { get; }
        public ICommand TimKiemCommand { get; }
        public ICommand ChonTatCaCommand { get; }
        public ICommand ChonThucAnCommand { get; }
        public ICommand ChonThucUongCommand { get; }

        public orderFood()
        {
            _orderService = new OrderService();

            DanhSachBanAn = _orderService.LayDanhSachBanAnTuDb();
            DanhSachMonAn = _orderService.LayDanhSachMonAnTuBd();
            DanhSachMonAnCoKM = _orderService.LayDanhSachMonAnKM();
            DanhSachHienThi = DanhSachMonAn;

            Messenger.Default.Register<BanAnUpdatedMessage>(this, msg =>
            {
                var ban = DanhSachBanAn?.FirstOrDefault(b => b.MaBan == msg.MaBan);
                if (ban != null)
                {
                    ban.TrangThai = msg.TrangThai;
                    var idx = DanhSachBanAn.IndexOf(ban);
                    DanhSachBanAn.RemoveAt(idx);
                    DanhSachBanAn.Insert(idx, ban);
                }
            });

            ChonBanCommand = new RelayCommand<BanAn>(ban =>
            {
                if (ban == null) return;
                BanDangChon = ban;
            });

            ThemMonCommand = new RelayCommand<MonAn>(mon =>
            {
                if (BanDangChon == null)
                {
                    MessageBox.Show("Vui lòng chọn bàn trước khi gọi món!", "Nhắc nhở");
                    return;
                }

                var monTrongGio = GioHang.FirstOrDefault(m => m.MaMonAn == mon.MaMonAn);
                if (monTrongGio != null)
                {
                    monTrongGio.SoLuong++;
                }
                else
                {
                    GioHang.Add(new MonAn
                    {
                        MaMonAn = mon.MaMonAn,
                        TenMonAn = mon.TenMonAn,
                        Gia = mon.Gia,
                        HinhAnhURL = mon.HinhAnhURL,
                        SoLuong = 1
                    });
                }
                OnPropertyChanged(nameof(TongTienGioHang));
            });

            TangSoLuongCommand = new RelayCommand<MonAn>(mon =>
            {
                if (mon != null)
                {
                    mon.SoLuong++;
                    OnPropertyChanged(nameof(TongTienGioHang));
                }
            });

            GiamSoLuongCommand = new RelayCommand<MonAn>(mon =>
            {
                if (mon != null)
                {
                    if (mon.SoLuong > 1)
                        mon.SoLuong--;
                    else
                        GioHang.Remove(mon);

                    OnPropertyChanged(nameof(TongTienGioHang));
                }
            });

            HuyDonHangCommand = new RelayCommand(() =>
            {
                if (GioHang.Count > 0)
                {
                    var result = MessageBox.Show("Bạn có chắc muốn hủy đơn hàng này?", "Xác nhận",
                        MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        GioHang.Clear();
                        OnPropertyChanged(nameof(TongTienGioHang));
                    }
                }
            });

            TimKiemCommand = new RelayCommand(() =>
            {
                if (string.IsNullOrWhiteSpace(TuKhoaTimKiem))
                    DanhSachHienThi = DanhSachMonAn;
                else
                    DanhSachHienThi = new ObservableCollection<MonAn>(
                        DanhSachMonAn.Where(m => m.TenMonAn.ToLower().Contains(TuKhoaTimKiem.ToLower())));
            });

            ChonTatCaCommand = new RelayCommand(() => DanhSachHienThi = DanhSachMonAn);
            ChonThucAnCommand = new RelayCommand(() =>
                DanhSachHienThi = new ObservableCollection<MonAn>(DanhSachMonAn.Where(m => m.Loai == 0)));
            ChonThucUongCommand = new RelayCommand(() =>
                DanhSachHienThi = new ObservableCollection<MonAn>(DanhSachMonAn.Where(m => m.Loai == 1)));

            XacNhanDatBanCommand = new RelayCommand(() => XacNhanDatBan(SessionUser.UserName));
        }

        private void XacNhanDatBan(string maNV)
        {
            if (BanDangChon == null)
            {
                MessageBox.Show("Vui lòng chọn bàn trước!", "Thông báo");
                return;
            }

            if (GioHang.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống! Vui lòng thêm món.", "Thông báo");
                return;
            }

            if (BanDangChon.TrangThai == "Đã đặt")
            {
                MessageBox.Show($"Bàn {BanDangChon.MaBan} đang có khách. Vui lòng chọn bàn trống!", "Thông báo");
                return;
            }

            try
            {
                // 1. Cập nhật trạng thái bàn
                bool capNhatBan = _orderService.CapNhatTrangThaiBan(BanDangChon.MaBan, "Đã đặt");
                if (!capNhatBan)
                {
                    MessageBox.Show("Lỗi khi cập nhật trạng thái bàn!", "Lỗi");
                    return;
                }

                // 2. Tạo đơn hàng mới
                var dh = new DonHang
                {
                    NgayDat = DateTime.Now,
                    MaBan = BanDangChon.MaBan,
                    MaNhanVien = string.IsNullOrEmpty(maNV) ? "admin" : maNV,
                    TrangThai = "DangXuLy",
                    TongTien = 0
                };

                int maDH = _orderService.ChenDonHang(dh);

                if (maDH <= 0)
                {
                    MessageBox.Show("Lỗi khi tạo đơn hàng!", "Lỗi");
                    _orderService.CapNhatTrangThaiBan(BanDangChon.MaBan, "Trống");
                    return;
                }

                // 3. Insert chi tiết đơn hàng
                foreach (var mon in GioHang)
                {
                    decimal giaGoc = Convert.ToDecimal(mon.Gia);
                    decimal soTienGiam = 0;
                    int? maKM = null;

                    // Kiểm tra khuyến mãi
                    if (DanhSachMonAnCoKM != null && DanhSachMonAnCoKM.Contains(mon.MaMonAn))
                    {
                        maKM = _orderService.LayMaKhuyenMaiCuaMonAn(mon.MaMonAn);
                        if (maKM.HasValue)
                        {
                            var km = _orderService.LayKhuyenMaiTheoMaKhuyenMai(maKM);
                            if (km != null)
                            {
                                if (km.LoaiGiamGia == "PhanTram")
                                    soTienGiam = (giaGoc * mon.SoLuong) * (km.GiaTriGiam / 100);
                                else if (km.LoaiGiamGia == "TienCoDinh")
                                    soTienGiam = km.GiaTriGiam;
                            }
                        }
                    }

                    decimal tienGoc = giaGoc * mon.SoLuong;
                    decimal thanhToanCuoi = tienGoc - soTienGiam;

                    _orderService.ChenChiTietDonHang(new ChiTietDonHang
                    {
                        MaDonHang = maDH,
                        MaMonAn = mon.MaMonAn,
                        SoLuong = mon.SoLuong,
                        MaKhuyenMai = maKM,
                        TienGoc = tienGoc,
                        SoTienGiam = soTienGiam,
                        ThanhToanCuoi = thanhToanCuoi
                    });
                }

                MessageBox.Show("Đặt món thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                // Cập nhật UI
                BanDangChon.TrangThai = "Đã đặt";
                Messenger.Default.Send(new BanAnUpdatedMessage(BanDangChon.MaBan, "Đã đặt"));

                GioHang.Clear();
                OnPropertyChanged(nameof(TongTienGioHang));
                BanDangChon = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Lỗi");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}