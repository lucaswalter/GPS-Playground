using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using GMap.NET;
using GMap.NET.WindowsPresentation;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using GMap.NET.MapProviders;
using GPS_Playground.Source;

namespace GPS_Playground
{
    public partial class MainWindow : Window
    {
        // Points
        PointLatLng start;
        PointLatLng end;

        // Marker
        GMapMarker currentMarker;

        public MainWindow()
        {
            InitializeComponent();

            // Switch to Cache mode if internet is not avaible
            if (!PingNetwork("pingtest.com"))
            {
                MainMap.Manager.Mode = AccessMode.CacheOnly;
                MessageBox.Show("No internet connection available, going to CacheOnly mode.", "GMap.NET - Demo.WindowsPresentation", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            // Basic Map Config
            MainMap.MapProvider = GMapProviders.OpenStreetMap;
            MainMap.Position = new PointLatLng(37.948544, -91.7715303);


            // Set Marker
            currentMarker = new GMapMarker(MainMap.Position);
            {
                currentMarker.Shape = new CustomMarkerRed(this, currentMarker, "Baller Rolla Marker");
                currentMarker.Offset = new System.Windows.Point(-15, -15);
                currentMarker.ZIndex = int.MaxValue;
                MainMap.Markers.Add(currentMarker);
            }
        }

        // TODO: DOES NOT BELONG HERE
        public static bool PingNetwork(string hostNameOrAddress)
        {
            bool pingStatus = false;

            using (Ping p = new Ping())
            {
                byte[] buffer = Encoding.ASCII.GetBytes("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
                int timeout = 4444; // 4s

                try
                {
                    PingReply reply = p.Send(hostNameOrAddress, timeout, buffer);
                    pingStatus = (reply.Status == IPStatus.Success);
                }
                catch (Exception)
                {
                    pingStatus = false;
                }
            }

            return pingStatus;
        }
    }
}
