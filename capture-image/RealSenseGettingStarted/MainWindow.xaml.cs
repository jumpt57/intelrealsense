using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace RealSenseGettingStarted
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            PXCMSession session = PXCMSession.CreateInstance();
            PXCMSession.ImplVersion version = session.QueryVersion();
            textBox1.Text = version.major.ToString() + "." + version.minor.ToString();
            PXCMSenseManager sm =  session.CreateSenseManager();

            sm.EnableStream(PXCMCapture.StreamType.STREAM_TYPE_COLOR, 0, 0);
            sm.EnableStream(PXCMCapture.StreamType.STREAM_TYPE_DEPTH, 0, 0);
            sm.Init();

            pxcmStatus status = sm.AcquireFrame(true);
            PXCMCapture.Sample sample = sm.QuerySample();

            PXCMImage image = sample.color;
            PXCMImage dimage = sample.depth;

            PXCMImage.ImageData data;
            PXCMImage.ImageData data2;

            image.AcquireAccess(PXCMImage.Access.ACCESS_READ, PXCMImage.PixelFormat.PIXEL_FORMAT_RGB32,  out data);
            dimage.AcquireAccess(PXCMImage.Access.ACCESS_READ, PXCMImage.PixelFormat.PIXEL_FORMAT_DEPTH_RAW, out data2);

            WriteableBitmap wbm = data.ToWritableBitmap(0, image.info.width, image.info.height, 96.0, 96.0);
            WriteableBitmap wbm2 = data2.ToWritableBitmap(0, dimage.info.width, dimage.info.height, 96.0, 96.0);

            image1.Source = wbm;
            image2.Source = wbm2;

            image.ReleaseAccess(data);
            dimage.ReleaseAccess(data2);
            sm.ReleaseFrame();
            sm.Close();
            session.Dispose();
        }
    }
}
