using restaurantManager.Services;
using System.Windows;

namespace restaurantManager
{
    public partial class Home : Window
    {
        public Home()
        {
            InitializeComponent();

            chkShowLoginPassword.Checked += (s, e) =>
                ScriptHome.TogglePassword(txtLoginPassword, txtShowPassword, true);
            chkShowLoginPassword.Unchecked += (s, e) =>
                ScriptHome.TogglePassword(txtLoginPassword, txtShowPassword, false);

            chkShowRegisterPassword.Checked += (s, e) =>
            {
                ScriptHome.TogglePassword(txtRegisterPassword, txtShowRegisterPassword, true);
                ScriptHome.TogglePassword(txtRegisterConfirm, txtShowRegisterConfirm, true);
            };
            chkShowRegisterPassword.Unchecked += (s, e) =>
            {
                ScriptHome.TogglePassword(txtRegisterPassword, txtShowRegisterPassword, false);
                ScriptHome.TogglePassword(txtRegisterConfirm, txtShowRegisterConfirm, false);
                MessageBox.Show("i");
            };
        }

        private void bthLogin_Click(object sender, RoutedEventArgs e)
        {
            string password = chkShowLoginPassword.IsChecked == true
                ? txtShowPassword.Text
                : txtLoginPassword.Password;

            ScriptHome.DangNhap(txtLoginUsername.Text.Trim(), password, this);
        }


        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (chkShowRegisterPassword.IsChecked == false)
            {
                ScriptHome.DangKy(txtRegisterUsername.Text.Trim(),
                                                  txtRegisterPassword.Password,
                                                  txtRegisterConfirm.Password);
            }
            else
            {
                ScriptHome.DangKy(txtRegisterUsername.Text.Trim(),
                                  txtShowRegisterPassword.Text,
                                  txtShowRegisterConfirm.Text);
            }
                
        }
    }
}
