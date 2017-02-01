using SondageInteractifv2.tools.window;
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
using System.Windows.Shapes;

namespace SondageInteractifv2.views.popups
{
    /// <summary>
    /// Logique d'interaction pour ErrorPopup.xaml
    /// </summary>
    public partial class ErrorPopup : Window
    {
        public ErrorPopup(string msg)
        {
            InitializeComponent();           
            HelperMethods.CenterWindowOnScreen(this);
            ErrorMsg.Text = msg;
            this.Topmost = true;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
