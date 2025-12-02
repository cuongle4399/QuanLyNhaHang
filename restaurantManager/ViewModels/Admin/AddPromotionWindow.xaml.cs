using System.Windows;
using restaurantManager.Models;
using restaurantManager.ViewModels;

namespace restaurantManager.View
{
    public partial class AddPromotionWindow : Window
    {
        public AddPromotionWindow()
        {
            InitializeComponent();
            DataContext = new AddPromotionViewModel();
        }

        public AddPromotionWindow(KhuyenMai promotion)
        {
            InitializeComponent();
            DataContext = new AddPromotionViewModel(promotion);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as AddPromotionViewModel;
            vm?.Save();
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}