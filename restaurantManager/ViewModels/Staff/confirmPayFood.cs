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
    public class confirmPayFood : INotifyPropertyChanged
    {
        // Giả sử tên class Service là ConfirmPayFoodService (sửa lại cho đúng tên trong project bạn)
        ComfirmPayFood _service;

        private ObservableCollection<BanAn> _danhSachBanAn;
        public ObservableCollection<BanAn> DanhSachBanAn
        {
            get => _danhSachBanAn;
            set { _danhSachBanAn = value; OnPropertyChanged(); }
        }

        private BanAn _banDangChon;
        public BanAn BanDangChon
        {
            get => _banDangChon;
            set { _banDangChon = value; OnPropertyChanged(); }
        }

        private decimal _tongTienPhaiThanhToan;
        public decimal TongTienPhaiThanhToan
        {
            get => _tongTienPhaiThanhToan;
            set { _tongTienPhaiThanhToan = value; OnPropertyChanged(); }
        }

        private DonHang _donHangCuaBan;
        public DonHang DonHangCuaBan
        {
            get => _donHangCuaBan;
            set { _donHangCuaBan = value; OnPropertyChanged(); }
        }

        private ObservableCollection<ChiTiet> _danhSachChiTietCuaDonHang;
        public ObservableCollection<ChiTiet> DanhSachChiTietCuaDonHang
        {
            get => _danhSachChiTietCuaDonHang;
            set { _danhSachChiTietCuaDonHang = value; OnPropertyChanged(); }
        }

        // SỬA LỖI LOGIC COMBOBOX
        private string _hinhThucThanhToan;
        public string HinhThucThanhToan
        {
            get => _hinhThucThanhToan;
            set
            {
                // Chỉ lưu giá trị hiển thị (VD: "💵 Tiền mặt") để UI hiển thị đúng
                _hinhThucThanhToan = value;
                OnPropertyChanged();
            }
        }

        public ICommand ChonBanCommand { get; }
        public ICommand HuyThanhToanCommand { get; }
        public ICommand XacNhanThanhToanCommand { get; }

        public confirmPayFood()
        {
            _service = new ComfirmPayFood();

            // Mặc định chọn tiền mặt
            HinhThucThanhToan = "💵 Tiền mặt";

            // Đăng ký lắng nghe cập nhật trạng thái bàn
            Messenger.Default.Register<BanAnUpdatedMessage>(this, msg =>
            {
                var ban = DanhSachBanAn?.FirstOrDefault(b => b.MaBan == msg.MaBan);
                if (ban != null)
                {
                    ban.TrangThai = msg.TrangThai;
                    // Force update UI nếu cần
                    var index = DanhSachBanAn.IndexOf(ban);
                    // Dòng này giúp cập nhật UI nếu BanAn không implement INotifyPropertyChanged hoặc nếu UI binding lỗi
                    // Nếu BanAn có INotifyPropertyChanged thì không cần dòng dưới này
                    // DanhSachBanAn[index] = ban; 
                }
            });

            DanhSachBanAn = _service.LayDanhSachBanAnTuDb();

            ChonBanCommand = new RelayCommand<BanAn>(ban =>
            {
                if (ban == null) return;

                foreach (var b in DanhSachBanAn) b.IsSelected = false;
                ban.IsSelected = true;
                BanDangChon = ban;

                // Load đơn hàng
                DonHangCuaBan = _service.LayDonHangMoiNhatTheoMaBan(ban.MaBan);

                if (DonHangCuaBan != null)
                {
                    DanhSachChiTietCuaDonHang = _service.LayBangChiTiet(BanDangChon.MaBan);
                    // Tính tổng tiền
                    TongTienPhaiThanhToan = 0;
                    if (DanhSachChiTietCuaDonHang != null)
                    {
                        foreach (var ct in DanhSachChiTietCuaDonHang)
                            TongTienPhaiThanhToan += ct.ThanhToanCuoi;
                    }
                }
                else
                {
                    DanhSachChiTietCuaDonHang = new ObservableCollection<ChiTiet>();
                    TongTienPhaiThanhToan = 0;
                }
            });

            HuyThanhToanCommand = new RelayCommand(() =>
            {
                DanhSachChiTietCuaDonHang?.Clear();
                BanDangChon = null;
                DonHangCuaBan = null;
                TongTienPhaiThanhToan = 0;

                // Reset selection UI
                foreach (var b in DanhSachBanAn) b.IsSelected = false;
            });

            XacNhanThanhToanCommand = new RelayCommand(() =>
            {
                if (BanDangChon == null || DonHangCuaBan == null)
                {
                    MessageBox.Show("Vui lòng chọn bàn có đơn hàng để thanh toán!", "Thông báo");
                    return;
                }

                // CONVERT hình thức thanh toán sang Code để lưu DB
                string paymentMethodCode = "TienMat"; // Default
                if (HinhThucThanhToan != null && HinhThucThanhToan.Contains("Chuyển khoản"))
                    paymentMethodCode = "ChuyenKhoan";

                bool ok1 = _service.CapNhatTrangThaiDonHang(DonHangCuaBan, "DaHoanThanh", TongTienPhaiThanhToan, paymentMethodCode);

                if (ok1)
                {
                    bool ok2 = _service.CapNhatTrangThaiBan(BanDangChon.MaBan, "Trống");
                    if (ok2)
                    {
                        // *** ĐIỀU CHỈNH LOGIC Ở ĐÂY ***

                        // 1. Cập nhật trạng thái bàn đang chọn trong ViewModel này
                        BanDangChon.TrangThai = "Trống"; // <-- Đảm bảo đối tượng hiện tại được cập nhật

                        MessageBox.Show("Thanh toán thành công!", "Thông báo");

                        // 2. Gửi tin nhắn cập nhật cho màn hình Order và các màn hình khác
                        Messenger.Default.Send(new BanAnUpdatedMessage(BanDangChon.MaBan, "Trống"));

                        // 3. Reset UI
                        HuyThanhToanCommand.Execute(null);

                        // 4. Bỏ reload toàn bộ list bàn, vì Messenger đã xử lý cập nhật.
                        // DanhSachBanAn = _service.LayDanhSachBanAnTuDb(); // <-- BỎ DÒNG NÀY
                    }
                    else
                    {
                        MessageBox.Show("Lỗi khi cập nhật trạng thái bàn!", "Lỗi");
                    }
                }
                else
                {
                    MessageBox.Show("Lỗi khi cập nhật đơn hàng!", "Lỗi");
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}