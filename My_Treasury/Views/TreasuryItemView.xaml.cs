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

namespace My_Treasury.Views
{
    /// <summary>
    /// Interaction logic for TreasuryItemView.xaml
    /// </summary>
    public partial class TreasuryItemView : UserControl
    {
        public TreasuryItemView()
        {
            InitializeComponent();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            userControl.Background = (SolidColorBrush)FindResource("HoverItemBGColor");
        }

        private void userControl_MouseLeave(object sender, MouseEventArgs e)
        {
            userControl.Background = (SolidColorBrush) FindResource("BlueBG");
        }
    }
}
