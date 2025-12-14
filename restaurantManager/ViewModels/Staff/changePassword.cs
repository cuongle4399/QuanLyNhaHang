using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
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

        public ICommand ToggleOldPasswordCommand { get; }
        public ICommand ToggleNewPasswordCommand { get; }
        public ICommand ToggleConfirmPasswordCommand { get; }
        public ICommand ChangePasswordCommand { get; }

        public ChangePasswordViewModel()
        {
            ToggleOldPasswordCommand = new RelayCommand(() => IsShowOld = !IsShowOld);
            ToggleNewPasswordCommand = new RelayCommand(() => IsShowNew = !IsShowNew);
            ToggleConfirmPasswordCommand = new RelayCommand(() => IsShowConfirm = !IsShowConfirm);
            ChangePasswordCommand = new RelayCommand(ChangePassword);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void ChangePassword()
        {
            // 1. Validate - Kiểm tra nhập đầy đủ
            if (string.IsNullOrWhiteSpace(OldPassword) ||
                string.IsNullOrWhiteSpace(NewPassword) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Kiểm tra mật khẩu cũ
            string currentPassword = Services.ChangePassword.LayMatKhauHienTai();
            if (string.IsNullOrEmpty(currentPassword))
            {
                MessageBox.Show("Không thể lấy thông tin mật khẩu hiện tại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (OldPassword.Trim() != currentPassword.Trim())
            {
                MessageBox.Show("Mật khẩu hiện tại không đúng.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 3. Kiểm tra mật khẩu mới phải khác mật khẩu cũ
            if (OldPassword.Trim() == NewPassword.Trim())
            {
                MessageBox.Show("Mật khẩu mới phải khác mật khẩu hiện tại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 4. Kiểm tra độ mạnh mật khẩu mới
            if (NewPassword.Length < 8 || !NewPassword.Any(char.IsLetter) || !NewPassword.Any(char.IsDigit))
            {
                MessageBox.Show("Mật khẩu mới phải có ít nhất 8 ký tự, bao gồm chữ và số.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 5. Kiểm tra xác nhận mật khẩu
            if (NewPassword != ConfirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 6. Cập nhật mật khẩu
            bool success = Services.ChangePassword.CapNhatMatKhau(NewPassword);
            if (success)
            {
                MessageBox.Show("Đổi mật khẩu thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                OldPassword = NewPassword = ConfirmPassword = "";
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra khi đổi mật khẩu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}