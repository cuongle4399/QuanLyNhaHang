using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using restaurantManager.Models;
using restaurantManager.Services;

using GalaSoft.MvvmLight.Command;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using restaurantManager.Messages;

namespace restaurantManager.ViewModels.Staff
{
    public class confirmPayFood : INotifyPropertyChanged
    {
        ComfirmPayFood _confirmPayFood;

        private ObservableCollection<BanAn> _danhSachBanAn;
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

        private BanAn _banDangChon;
        public BanAn BanDangChon
        {
            get => _banDangChon;
            set
            {
                if (_banDangChon != value)
                {
                    _banDangChon = value;
                    OnPropertyChanged();
                }
            }
        }


        private decimal _tongTienPhaiThanhToan;
        public decimal TongTienPhaiThanhToan
        {
            get => _tongTienPhaiThanhToan;
            set
            {
                _tongTienPhaiThanhToan = value;
                OnPropertyChanged();
            }
        }
        public void TinhTongDonHang(ObservableCollection<ChiTiet> dsChiTiet)
        {
            TongTienPhaiThanhToan = 0;

            foreach (ChiTiet ct in dsChiTiet)
            {
                TongTienPhaiThanhToan += ct.ThanhToanCuoi;
            }
        }

        private DonHang _donHangCuaBan;
        public DonHang DonHangCuaBan
        {
            get => _donHangCuaBan;
            set
            {
                _donHangCuaBan = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ChiTiet> _danhSachChiTietCuaDonHang;
        public ObservableCollection<ChiTiet> DanhSachChiTietCuaDonHang
        {
            get => _danhSachChiTietCuaDonHang;
            set
            {
                _danhSachChiTietCuaDonHang = value;
                OnPropertyChanged();
            }
        }

        public ICommand ChonBanCommand { get; }

        public ICommand HuyThanhToanCommand { get; }

        public ICommand XacNhanThanhToanCommand { get; }


        public confirmPayFood()
        {

            _confirmPayFood = new ComfirmPayFood();

            // ✅ Đăng ký lắng nghe cập nhật từ orderFood
            Messenger.Default.Register<BanAnUpdatedMessage>(this, msg =>
            {
                var ban = DanhSachBanAn.FirstOrDefault(b => b.MaBan == msg.MaBan);
                if (ban != null)
                {
                    ban.TrangThai = msg.TrangThai;
                    OnPropertyChanged(nameof(DanhSachBanAn));
                }
            });

            // Load tất cả bàn ăn khi khởi tạo
            DanhSachBanAn = _confirmPayFood.LayDanhSachBanAnTuDb();

            ChonBanCommand = new RelayCommand<BanAn>(ban =>
            {
                if (ban == null) return;

                foreach (var b in DanhSachBanAn)
                    b.IsSelected = false;

                ban.IsSelected = true;
                BanDangChon = ban;

                DonHangCuaBan = _confirmPayFood.LayDonHangMoiNhatTheoMaBan(ban.MaBan);

                if (DonHangCuaBan != null)
                    DanhSachChiTietCuaDonHang = _confirmPayFood.LayBangChiTiet(BanDangChon.MaBan);
                else
                    DanhSachChiTietCuaDonHang = new ObservableCollection<ChiTiet>();

                TinhTongDonHang(DanhSachChiTietCuaDonHang);
            });


            HuyThanhToanCommand = new RelayCommand(() =>
            {
                if (DanhSachChiTietCuaDonHang != null)
                    DanhSachChiTietCuaDonHang.Clear();
                if (BanDangChon != null) BanDangChon = null;
                if (DonHangCuaBan != null) DonHangCuaBan = null;
                if (BanDangChon != null) BanDangChon.IsSelected = false;
            });

            // ✅ Xác nhận thanh toán
            XacNhanThanhToanCommand = new RelayCommand(() =>
            {
                if (BanDangChon == null || DonHangCuaBan == null)
                {
                    MessageBox.Show("Vui lòng chọn bàn và đơn hàng trước khi xác nhận thanh toán!");
                    return;
                }

                // Cập nhật trạng thái đơn hàng
                bool ok1 = _confirmPayFood.CapNhatTrangThaiDonHang(DonHangCuaBan, "DaHoanThanh", TongTienPhaiThanhToan);
                // Cập nhật trạng thái bàn
                bool ok2 = _confirmPayFood.CapNhatTrangThaiBan(BanDangChon.MaBan, "Trống");

                if (ok1 && ok2)
                {
                    MessageBox.Show("Thanh toán thành công!");

                    // Gửi message để orderFood cập nhật lại trạng thái bàn
                    Messenger.Default.Send(new BanAnUpdatedMessage(BanDangChon.MaBan, "Trống"));

                    // Reset UI
                    DonHangCuaBan = null;
                    DanhSachChiTietCuaDonHang?.Clear();
                    BanDangChon = null;

                    // Reload danh sách bàn để UI hiển thị lại
                    DanhSachBanAn = _confirmPayFood.LayDanhSachBanAnTuDb();
                }
                else
                {
                    MessageBox.Show("Thanh toán thất bại, vui lòng thử lại!");
                }

                BanDangChon = null;
                DonHangCuaBan = null;
                DanhSachChiTietCuaDonHang = null;
            });
        }



        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
