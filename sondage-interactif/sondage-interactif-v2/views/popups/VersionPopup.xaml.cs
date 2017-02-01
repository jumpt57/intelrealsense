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
    /// Logique d'interaction pour VersionPopup.xaml
    /// </summary>
    public partial class VersionPopup : Window
    {
        public VersionPopup()
        {
            InitializeComponent();
            HelperMethods.CenterWindowOnScreen(this);
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();            
        }
    }
}
