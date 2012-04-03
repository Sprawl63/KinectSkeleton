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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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

            }
            /*
            using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
            {
                if (depthFrame == null)
                {
                    return;
                }

            }
             */

        }

        void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {

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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}
