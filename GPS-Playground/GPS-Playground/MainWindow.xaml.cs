using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;
using GPS_Playground.Source;
using System;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows;

namespace GPS_Playground
{
    public partial class MainWindow : Window
    {
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

        public void PlaceMarker(double latitude, double longitude)
        {
            PointLatLng tempPosition = new PointLatLng(latitude, longitude);
            GMapMarker tempMarker = new GMapMarker(tempPosition);

            // Set Position On Map
            tempMarker.Shape = new CustomMarkerRed(this, tempMarker, "Marker");
            tempMarker.Offset = new System.Windows.Point(-15, -15);
            tempMarker.ZIndex = int.MaxValue;
            MainMap.Markers.Add(tempMarker);
        }

        private void ImportBtn_Click(object sender, RoutedEventArgs e)
        {
            MainMap.ShowImportDialog();
        }

        private void ExportBtn_Click(object sender, RoutedEventArgs e)
        {
            MainMap.ShowExportDialog();
        }

        private void PrefetchBtn_Click(object sender, RoutedEventArgs e)
        {
            RectLatLng area = MainMap.SelectedArea;
            if (!area.IsEmpty)
            {
                for (int i = (int)MainMap.Zoom; i <= MainMap.MaxZoom; i++)
                {
                    MessageBoxResult res = MessageBox.Show("Ready ripp at Zoom = " + i + " ?", "GMap.NET", MessageBoxButton.YesNoCancel);

                    if (res == MessageBoxResult.Yes)
                    {
                        TilePrefetcher obj = new TilePrefetcher();
                        obj.Owner = this;
                        obj.ShowCompleteMessage = true;
                        obj.Start(area, i, MainMap.MapProvider, 100);
                    }
                    else if (res == MessageBoxResult.No)
                    {
                        continue;
                    }
                    else if (res == MessageBoxResult.Cancel)
                    {
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Select map area holding ALT", "GMap.NET", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are You sure?", "Clear GMap.NET cache?", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK)
            {
                try
                {
                    MainMap.Manager.PrimaryCache.DeleteOlderThan(DateTime.Now, null);
                    MessageBox.Show("Done. Cache is clear.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
