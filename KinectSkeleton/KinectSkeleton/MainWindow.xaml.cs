using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.Timers;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        KinectSensor _sensor;
        int playerDepth;
        int lastnum = 10;
        int num = 1;
        Boolean timetoflip = true;
        static Random rdm = new Random(3);
        BitmapImage image = new BitmapImage(new Uri("/Images/S1.png", UriKind.Relative));

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
             //initialize and start my timer with a tick time of 15sec
              Timer myTimer = new Timer();
              myTimer.Elapsed += new ElapsedEventHandler( DisplayTimeEvent );
              myTimer.Interval = 2000;
              myTimer.Start();
           
            
            if (KinectSensor.KinectSensors.Count > 0)
            {
                _sensor = KinectSensor.KinectSensors[0];

                if (_sensor.Status == KinectStatus.Connected)
                {
                    _sensor.ColorStream.Enable();
                    _sensor.DepthStream.Enable();
                    _sensor.SkeletonStream.Enable();
                    _sensor.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(_sensor_AllFramesReady);
                    _sensor.Start();
                }
            }

        }
        public void DisplayTimeEvent( object source, ElapsedEventArgs e )
         {
             if (timetoflip==true)
             {
                 num = rdm.Next(3);
                 while (lastnum == num)
                 {
                     num = rdm.Next(3);
                 }
             }
             
             
         }
        void _sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            
            //throw new NotImplementedException();
            using (ColorImageFrame colorFrame = e.OpenColorImageFrame())
            {
                if (colorFrame == null)
                {
                    return;
                }

                byte[] pixels = new byte[colorFrame.PixelDataLength];

                //copy data out into our byte array
                colorFrame.CopyPixelDataTo(pixels);

                int stride = colorFrame.Width * 4;

                image1.Source = BitmapSource.Create(colorFrame.Width, colorFrame.Height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);
                
                if (num == 2)
                {
                    image2.Source = new BitmapImage(new Uri("/Images/S3.png", UriKind.Relative));
                    lastnum = 2;
                }
                else if (num == 1)
                {
                    image2.Source = new BitmapImage(new Uri("/Images/S2.png", UriKind.Relative));
                    lastnum = 1;
                }
                else
                {
                    image2.Source = new BitmapImage(new Uri("/Images/S1.png", UriKind.Relative));
                    lastnum = 0;
                }
            }
        }

        void GetCameraPoint(Skeleton first, AllFramesReadyEventArgs e)
        {
            using (DepthImageFrame depth = e.OpenDepthImageFrame())
            {
                if (depth == null ||
                    _sensor == null)
                {
                    return;
                }


                //Map a joint location to a point on the depth map
                //head
                DepthImagePoint headDepthPoint =
                    depth.MapFromSkeletonPoint(first.Joints[JointType.Head].Position);
                //left hand
                DepthImagePoint leftHandDepthPoint =
                    depth.MapFromSkeletonPoint(first.Joints[JointType.HandLeft].Position);
                //right hand
                DepthImagePoint rightHandDepthPoint =
                    depth.MapFromSkeletonPoint(first.Joints[JointType.HandRight].Position);
                //left foot
                DepthImagePoint leftFootDepthPoint =
                    depth.MapFromSkeletonPoint(first.Joints[JointType.HandRight].Position);
                //right foot
                DepthImagePoint rightFootDepthPoint =
                    depth.MapFromSkeletonPoint(first.Joints[JointType.HandRight].Position);
                //hip
                DepthImagePoint hipDepthPoint =
                    depth.MapFromSkeletonPoint(first.Joints[JointType.HipCenter].Position);



                //Map a depth point to a point on the color image
                //head
                ColorImagePoint headColorPoint =
                    depth.MapToColorImagePoint(headDepthPoint.X, headDepthPoint.Y,
                    ColorImageFormat.RgbResolution640x480Fps30);
                //left hand
                ColorImagePoint leftHandColorPoint =
                    depth.MapToColorImagePoint(leftHandDepthPoint.X, leftHandDepthPoint.Y,
                    ColorImageFormat.RgbResolution640x480Fps30);
                //right hand
                ColorImagePoint rightHandColorPoint =
                    depth.MapToColorImagePoint(rightHandDepthPoint.X, rightHandDepthPoint.Y,
                    ColorImageFormat.RgbResolution640x480Fps30);
                //left foot
                ColorImagePoint leftFootColorPoint =
                    depth.MapToColorImagePoint(leftFootDepthPoint.X, leftFootDepthPoint.Y,
                    ColorImageFormat.RgbResolution640x480Fps30);
                //right foot
                ColorImagePoint rightFootColorPoint =
                    depth.MapToColorImagePoint(rightFootDepthPoint.X, rightFootDepthPoint.Y,
                    ColorImageFormat.RgbResolution640x480Fps30);
                //hip
                ColorImagePoint hipColorPoint =
                    depth.MapToColorImagePoint(hipDepthPoint.X, hipDepthPoint.Y,
                    ColorImageFormat.RgbResolution640x480Fps30);
            }
        }

        private void StopKinect(KinectSensor sensor)    
        {
            if (sensor != null)
            {
                if (sensor.IsRunning)
                {
                    //stop sensor 
                    sensor.Stop();

                    //stop audio if not null
                    if (sensor.AudioSource != null)
                    {
                        sensor.AudioSource.Stop();
                    }
                }
            }
        }
        /**
        * JOINT XY Coordinates for Image 1-3
        * 
        * S3 Points
        *Head:278,102
        *LeftHand:85,167
        *RightHand:468,145
        *RightFoot:376,385
        *LeftFoot:105,373
        *HipCenter:275,239

        *S2 Points
        *Head:239,76
        *LeftHand:156,115
        *RightHand:328,206
        *RightFoot:305,312
        *LeftFoot:280,385
        *HipCenter:259,222

        *S1 Points
        *Head:242,66
        *LeftHand:239,0
        *RightHand:385,31
        *RightFoot:252,385
        *LeftFoot:212,385
        *HipCenter:234,210
        **/
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

    }
}
