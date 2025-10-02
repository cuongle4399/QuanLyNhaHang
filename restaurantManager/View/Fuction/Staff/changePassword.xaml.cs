using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using restaurantManager.ViewModels.Staff; // để dùng ChangePasswordViewModel

namespace restaurantManager.View.Fuction.Staff
{
    public partial class changePassword : UserControl
    {
        private ChangePasswordViewModel _vm;

        public changePassword()
        {
            InitializeComponent();

            if (this.DataContext == null)
                this.DataContext = new ChangePasswordViewModel();

            this.Loaded += ChangePassword_Loaded;
            this.Unloaded += ChangePassword_Unloaded;
        }

        private void ChangePassword_Loaded(object sender, RoutedEventArgs e)
        {
            AttachVm(this.DataContext as ChangePasswordViewModel);
        }

        private void ChangePassword_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachVm();
        }

        private void AttachVm(ChangePasswordViewModel vm)
        {
            if (_vm == vm) return;
            DetachVm();
            _vm = vm;
            if (_vm != null)
            {
                _vm.PropertyChanged += Vm_PropertyChanged;

                // khởi tạo PasswordBox từ VM (nếu VM đã có giá trị)
                pwdOld.Password = _vm.OldPassword ?? string.Empty;
                pwdNew.Password = _vm.NewPassword ?? string.Empty;
                pwdConfirm.Password = _vm.ConfirmPassword ?? string.Empty;
            }
        }

        private void DetachVm()
        {
            if (_vm != null)
            {
                _vm.PropertyChanged -= Vm_PropertyChanged;
                _vm = null;
            }
        }

        private void Vm_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Khi ViewModel thay đổi password (ví dụ do programmatically),
            // cập nhật PasswordBox tương ứng (tránh gõ ngược vòng lặp)
            if (_vm == null) return;

            switch (e.PropertyName)
            {
                case nameof(ChangePasswordViewModel.OldPassword):
                    if (pwdOld.Password != _vm.OldPassword)
                        pwdOld.Dispatcher.Invoke(() => pwdOld.Password = _vm.OldPassword ?? string.Empty);
                    break;

                case nameof(ChangePasswordViewModel.NewPassword):
                    if (pwdNew.Password != _vm.NewPassword)
                        pwdNew.Dispatcher.Invoke(() => pwdNew.Password = _vm.NewPassword ?? string.Empty);
                    break;

                case nameof(ChangePasswordViewModel.ConfirmPassword):
                    if (pwdConfirm.Password != _vm.ConfirmPassword)
                        pwdConfirm.Dispatcher.Invoke(() => pwdConfirm.Password = _vm.ConfirmPassword ?? string.Empty);
                    break;
            }
        }

        // Khi user gõ trong PasswordBox -> cập nhật ViewModel
        private void PwdOld_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ChangePasswordViewModel vm)
            {
                if (vm.OldPassword != pwdOld.Password)
                    vm.OldPassword = pwdOld.Password;
            }
        }

        private void PwdNew_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ChangePasswordViewModel vm)
            {
                if (vm.NewPassword != pwdNew.Password)
                    vm.NewPassword = pwdNew.Password;
            }
        }

        private void PwdConfirm_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is ChangePasswordViewModel vm)
            {
                if (vm.ConfirmPassword != pwdConfirm.Password)
                    vm.ConfirmPassword = pwdConfirm.Password;
            }
        }
    }

    // =========== Converters ở cấp namespace (XAML có thể tìm thấy) ===========

    // Converter: bool -> Visibility (ngược)
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool b = false;
            if (value is bool) b = (bool)value;
            return b ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Visibility) return (Visibility)value != Visibility.Visible;
            return false;
        }
    }

    // Converter: bool -> biểu tượng mắt (👁 / 🙈)
    public class BoolToEyeIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isShow = false;
            if (value is bool) isShow = (bool)value;
            return isShow ? "🙈" : "👁";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => throw new NotImplementedException();
    }
}
