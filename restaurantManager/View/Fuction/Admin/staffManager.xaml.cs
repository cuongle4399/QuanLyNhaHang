using restaurantManager.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using restaurantManager.ViewModels.Admin;

namespace restaurantManager.View.Fuction.Admin
{
    /// <summary>
    /// Interaction logic for staffManager.xaml
    /// </summary>
    public partial class staffManager : UserControl
    {
        private StaffManager vm;

        public staffManager()
        {
            InitializeComponent();

            vm = new StaffManager();
            this.DataContext = vm;
            DgNhanVien.ItemsSource = vm.DanhSachNhanVien;
        }

        private void DgNhanVien_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DgNhanVien.SelectedItem is NhanVien nv)
            {
                txtMaNhanVien.Text = nv.MaNhanVien.ToString();
                txtHoTen.Text = nv.HoTen;
                txtChucVu.Text = nv.ChucVu;
                txtLuong.Text = nv.Luong.ToString();
                txtSoDienThoai.Text = nv.SoDienThoai;
                txtDiaChi.Text = nv.DiaChi;
                dpNgaySinh.SelectedDate = nv.NgaySinh;

                if (!string.IsNullOrEmpty(nv.TrangThai) && nv.TrangThai.ToLower().Contains("nghỉ"))
                    cbTrangThai.SelectedIndex = 1;
                else
                    cbTrangThai.SelectedIndex = 0;
            }
        }

        private void Them_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtHoTen.Text) ||
                    string.IsNullOrWhiteSpace(txtChucVu.Text) ||
                    string.IsNullOrWhiteSpace(txtSoDienThoai.Text) ||
                    string.IsNullOrWhiteSpace(txtDiaChi.Text) ||
                    string.IsNullOrWhiteSpace(txtLuong.Text) ||
                    cbTrangThai.SelectedItem == null ||
                    dpNgaySinh.SelectedDate == null)
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin nhân viên!",
                                    "Thiếu thông tin", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(txtLuong.Text, out decimal luong) || luong < 0)
                {
                    MessageBox.Show("Lương phải là số và >= 0!", "Lỗi nhập liệu",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int soThuTu = vm.DanhSachNhanVien.Count + 1;
                string maNV = "NV" + soThuTu;

                var nv = new NhanVien
                {
                    MaNhanVien = maNV,
                    HoTen = txtHoTen.Text,
                    ChucVu = txtChucVu.Text,
                    SoDienThoai = txtSoDienThoai.Text,
                    DiaChi = txtDiaChi.Text,
                    NgaySinh = dpNgaySinh.SelectedDate ?? DateTime.Now,
                    Luong = luong,
                    TrangThai = (cbTrangThai.SelectedItem as ComboBoxItem)?.Content.ToString()
                };

                string matKhauDefault = "1";

                StaffManager.InsertFullData(nv, matKhauDefault);
                vm.DanhSachNhanVien.Add(nv);

                MessageBox.Show("Thêm nhân viên thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm nhân viên: " + ex.Message);
            }
        }


        private void Sua_Click_1(object sender, RoutedEventArgs e)
        {
            if (DgNhanVien.SelectedItem is NhanVien nv)
            {
                try
                {
                    nv.HoTen = txtHoTen.Text;
                    nv.ChucVu = txtChucVu.Text;
                    nv.SoDienThoai = txtSoDienThoai.Text;
                    nv.DiaChi = txtDiaChi.Text;
                    nv.NgaySinh = dpNgaySinh.SelectedDate ?? DateTime.Now;
                    nv.Luong = decimal.TryParse(txtLuong.Text, out decimal luong) ? luong : 0;
                    nv.TrangThai = (cbTrangThai.SelectedItem as ComboBoxItem)?.Content.ToString();

                    StaffManager.UpdateData(nv); 
                    MessageBox.Show("Cập nhật nhân viên thành công!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi sửa nhân viên: " + ex.Message);
                }
            }
        }

        private void Xoa_Click_2(object sender, RoutedEventArgs e)
        {
            if (DgNhanVien.SelectedItem is NhanVien nv)
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa nhân viên này?",
                                    "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Warning)
                    == MessageBoxResult.Yes)
                {
                    try
                    {
                        StaffManager.DeleteData(nv.MaNhanVien);
                        vm.DanhSachNhanVien.Remove(nv);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa nhân viên: " + ex.Message);
                    }
                }
            }
        }
    }
}
