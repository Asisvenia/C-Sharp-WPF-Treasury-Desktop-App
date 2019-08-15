using My_Treasury.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace My_Treasury
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TreasuryViewModel TreasuryVM;

        public MainWindow()
        {
            InitializeComponent();
            TreasuryVM = new TreasuryViewModel(this);
            DataContext = TreasuryVM;
        }
        private void currencyBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           TreasuryVM.GetSelectedCurrencyValue();
           TreasuryVM.SaveSelectedCurrency();
        }
        public void SelectLastSelectedItem(int index)
        {
            currencyBox.SelectedIndex = index;
        }
        /// Set selected item of currencybox
        public void SelectFirstItem()
        {
            currencyBox.SelectedIndex = 0;
        }
        public void SelectLastAvailableItem(int value)
        {
            currencyBox.SelectedIndex = value;
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TreasuryVM.IsDeleteItemBtnEnabled = Visibility.Visible;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
