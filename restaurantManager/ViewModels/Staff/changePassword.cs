using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command; // RelayCommand

using restaurantManager.Services;

namespace restaurantManager.ViewModels.Staff
{
    public class ChangePasswordViewModel : INotifyPropertyChanged
    {
        private string _oldPassword;
        private string _newPassword;
        private string _confirmPassword;

        private bool _isShowOld;
        private bool _isShowNew;
        private bool _isShowConfirm;

        public string OldPassword { get => _oldPassword; set { _oldPassword = value; OnPropertyChanged(); } }
        public string NewPassword { get => _newPassword; set { _newPassword = value; OnPropertyChanged(); } }
        public string ConfirmPassword { get => _confirmPassword; set { _confirmPassword = value; OnPropertyChanged(); } }

        public bool IsShowOld { get => _isShowOld; set { _isShowOld = value; OnPropertyChanged(); } }
        public bool IsShowNew { get => _isShowNew; set { _isShowNew = value; OnPropertyChanged(); } }
        public bool IsShowConfirm { get => _isShowConfirm; set { _isShowConfirm = value; OnPropertyChanged(); } }

        // Commands để toggle show/hide
        public ICommand ToggleOldPasswordCommand { get; }
        public ICommand ToggleNewPasswordCommand { get; }
        public ICommand ToggleConfirmPasswordCommand { get; }

        public ICommand ChangePasswordCommand { get; }

        public ChangePasswordViewModel()
        {
            // RelayCommand của MVVM Light hỗ trợ Action không tham số
            ToggleOldPasswordCommand = new RelayCommand(() => IsShowOld = !IsShowOld);
            ToggleNewPasswordCommand = new RelayCommand(() => IsShowNew = !IsShowNew);
            ToggleConfirmPasswordCommand = new RelayCommand(() => IsShowConfirm = !IsShowConfirm);

            ChangePasswordCommand = new RelayCommand(ChangePassword); // gắn method xử lý
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // ===== Xử lý khi nhấn "Đổi mật khẩu" =====
        private void ChangePassword()
        {
            // 1. Validate
            if (string.IsNullOrWhiteSpace(OldPassword) ||
                string.IsNullOrWhiteSpace(NewPassword) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NewPassword.Length < 8 || !NewPassword.Any(char.IsLetter) || !NewPassword.Any(char.IsDigit))
            {
                MessageBox.Show("Mật khẩu mới phải ≥ 8 ký tự, gồm chữ và số.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Kiểm tra mật khẩu cũ (giả sử bạn có hàm CheckOldPassword trong service)
            bool correctOld = CheckOldPassword(OldPassword, Services.ChangePassword.LayMatKhauHienTai());
            if (!correctOld)
            {

                MessageBox.Show("Mật khẩu hiện tại không đúng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 3. Cập nhật mật khẩu (gọi DB/service)
            bool success = Services.ChangePassword.CapNhatMatKhau(NewPassword);
            if (success)
            {
                MessageBox.Show("Đổi mật khẩu thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                OldPassword = NewPassword = ConfirmPassword = ""; // reset field
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra khi đổi mật khẩu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool CheckOldPassword(string OldPassword, string NewPassword)
        {
            return OldPassword.Trim().Equals(NewPassword.Trim());
        }

        private void OnPropertyChanged([CallerMemberName] string propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
