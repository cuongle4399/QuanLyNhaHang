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
using GalaSoft.MvvmLight.Messaging;
using restaurantManager.Messages;

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

        public ICommand GiamSoLuongCommand { get; }

        public ICommand XacNhanDatBanCommand { get; }
        public ICommand TimKiemCommand { get; }

        private readonly OrderService _orderService;

        public orderFood()
        {
            _orderService = new OrderService();

            // ✅ Đăng ký lắng nghe cập nhật từ confirmPayFood
            Messenger.Default.Register<BanAnUpdatedMessage>(this, msg =>
            {
                var ban = DanhSachBanAn.FirstOrDefault(b => b.MaBan == msg.MaBan);
                if (ban != null)
                {
                    ban.TrangThai = msg.TrangThai;
                    OnPropertyChanged(nameof(DanhSachBanAn));
                }
            });

            LoaiDangChon = -1;
            ChonTatCaCommand = new RelayCommand(() => LoaiDangChon = -1);
            ChonThucAnCommand = new RelayCommand(() => LoaiDangChon = 0);
            ChonThucUongCommand = new RelayCommand(() => LoaiDangChon = 1);

            HuyDonHangCommand = new RelayCommand(() => { GioHang.Clear(); OnPropertyChanged(nameof(TongTienGioHang)); });

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

            TimKiemCommand = new RelayCommand(() =>
            {
                if (string.IsNullOrWhiteSpace(TuKhoaTimKiem))
                {
                    // Nếu rỗng thì hiển thị tất cả
                    DanhSachHienThi = DanhSachMonAn;
                }
                else
                {
                    // Lọc danh sách theo từ khóa
                    DanhSachHienThi = new ObservableCollection<MonAn>(
                        DanhSachMonAn.Where(m =>
                            m.TenMonAn.Contains(TuKhoaTimKiem, StringComparison.OrdinalIgnoreCase) ||
                            m.MoTa.Contains(TuKhoaTimKiem, StringComparison.OrdinalIgnoreCase))
                    );
                }
            });

            XacNhanDatBanCommand = new RelayCommand(() => XacNhanDatBan(SessionUser.UserName));


            DanhSachBanAn = _orderService.LayDanhSachBanAnTuDb();
            DanhSachMonAn = _orderService.LayDanhSachMonAnTuBd();

            DanhSachMonAnCoKM = _orderService.LayDanhSachMonAnKM();

            LocDanhSachTheoLoai();
        }

        private string _tuKhoaTimKiem;
        public string TuKhoaTimKiem
        {
            get => _tuKhoaTimKiem;
            set
            {
                if (_tuKhoaTimKiem != value)
                {
                    _tuKhoaTimKiem = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<int> _danhSachMonAnCoKM;
        public ObservableCollection<int> DanhSachMonAnCoKM
        {
            get => _danhSachMonAnCoKM;
            set
            {
                _danhSachMonAnCoKM = value;
                OnPropertyChanged();
            }
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
            set
            {
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


            foreach (var mon in GioHang)
            {

                decimal soTienGiam = 0;
                int? maKM = null;
                KhuyenMai khuyenMai;

                if (DanhSachMonAnCoKM.Contains(mon.MaMonAn)) // Có khuyến mãi
                {
                    maKM = _orderService.LayMaKhuyenMaiCuaMonAn(mon.MaMonAn);

                    khuyenMai = _orderService.LayKhuyenMaiTheoMaKhuyenMai(maKM);

                    if (khuyenMai.LoaiGiamGia == "PhanTram") // Giảm theo phần trăm
                    {
                        soTienGiam = (Convert.ToDecimal(mon.Gia) * mon.SoLuong) * (khuyenMai.GiaTriGiam / 100);
                    }
                    else if (khuyenMai.LoaiGiamGia == "TienCoDinh")
                    {
                        soTienGiam = khuyenMai.GiaTriGiam;
                    }
                }

                bool ok = _orderService.ChenChiTietDonHang(new ChiTietDonHang
                {
                    MaDonHang = maDH,
                    MaMonAn = mon.MaMonAn,
                    SoLuong = mon.SoLuong,
                    MaKhuyenMai = maKM,
                    TienGoc = Convert.ToDecimal(mon.Gia) * mon.SoLuong,
                    SoTienGiam = soTienGiam,
                    ThanhToanCuoi = (Convert.ToDecimal(mon.Gia) * mon.SoLuong) - soTienGiam
                });

                if (!ok)
                {
                    MessageBox.Show("Đặt hàng thất bại.");
                    return;
                }
            }

            selectedBan.TrangThai = "Đã đặt";
            GioHang.Clear();
            OnPropertyChanged(nameof(TongTienGioHang));
            ThongBao = "Xác nhận đặt bàn thành công!";

            // Gửi message để confirmPayFood cập nhật
            Messenger.Default.Send(new BanAnUpdatedMessage(selectedBan.MaBan, "Đã đặt"));
        }


        private void LocDanhSachTheoLoai()
        {
            if (LoaiDangChon == -1)
            {
                DanhSachHienThi = DanhSachMonAn;
            }
            else
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
