using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NavigationALaMain
{
    /// <summary>
    /// Logique d'interaction pour TimedPopUp.xaml
    /// </summary>
    public partial class TimedPopUp : Window
    {
        private Timer popUpTimer;
        public TimedPopUp(string title, string message, TimeSpan timeToLive)
        {
            InitializeComponent();
            Title = title;
            PopUpTextBoxText.Text = message;
            //We subtract 750 milliseconds to allow the pop up to fade out before closing
            popUpTimer = new Timer(Math.Max(0, timeToLive.TotalMilliseconds - 750));
            popUpTimer.Elapsed += PopUpTimerOnElapsed;
            popUpTimer.Start();
            CenterWindowOnScreen();
        }

        public static bool IsWindowOpen<T>(string name = "") where T : Window
        {
            return string.IsNullOrEmpty(name)
               ? Application.Current.Windows.OfType<T>().Any()
               : Application.Current.Windows.OfType<T>().Any(w => w.Name.Equals(name));
        }

        private void PopUpTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Dispatcher.Invoke(new Action(Close), null);

        }

        public string Message
        {
            get { return PopUpTextBoxText.Text; }
            set { Dispatcher.BeginInvoke((Action)(() => UpdatePopUpText(value))); }
        }

        private void UpdatePopUpText(string value)
        {
            PopUpTextBoxText.Text = value;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Closing -= Window_Closing;
            e.Cancel = true;
            var anim = new DoubleAnimation(0, TimeSpan.FromSeconds(1));
            anim.Completed += (s, _) => Close();
            this.BeginAnimation(OpacityProperty, anim);
        }

        #region Permet de centrer la fenêtre au démarrage
        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }
        #endregion
    }
}
