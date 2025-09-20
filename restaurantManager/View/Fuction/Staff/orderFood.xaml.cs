using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using restaurantManager.Models;

namespace restaurantManager.View.Fuction.Staff
{
    /// <summary>
    /// Interaction logic for orderFood.xaml
    /// </summary>
    public partial class orderFood : UserControl
    {
        ViewModels.Staff.orderFood vm;

        public orderFood()
        {
            InitializeComponent();

            vm = new ViewModels.Staff.orderFood();

            this.DataContext = vm;

            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(vm.ThongBao) && !string.IsNullOrEmpty(vm.ThongBao))
                {
                    MessageBox.Show(vm.ThongBao);
                }
            };

        }


    }
}
