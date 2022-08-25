using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestArcgis
{
    public partial class MainPage : ContentPage
    {
        private Common.EsriMapComponent mArcgisMap;
        private Xamarin.Essentials.GeolocationRequest mRequest = null;
        private Xamarin.Essentials.Location mLastLocation = null;
        public MainPage()
        {
            InitializeComponent();
            mArcgisMap = new Common.EsriMapComponent(true);
            xGrid.Children.Add(mArcgisMap.GetMapView());
            mArcgisMap.CreateMap();
            mArcgisMap.GetReRoutedCallBack(NavigateReRoutedCallBack);
            GetGPSLocationAsync();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (mLastLocation != null)
            {
                mArcgisMap.RoutedAddress(mArcgisMap.GetWgs84MapPoint(36.392415966496, 127.31265386872), mArcgisMap.GetWgs84MapPoint(mLastLocation.Latitude, mLastLocation.Longitude), RoutedCatchCallBack);
            }
        }

        private void GetGPSLocationAsync()
        {
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(async () =>
            {
                mRequest = new Xamarin.Essentials.GeolocationRequest(Xamarin.Essentials.GeolocationAccuracy.Best, TimeSpan.FromSeconds(5));

                GetLocationData(await Xamarin.Essentials.Geolocation.GetLocationAsync(mRequest));
            });
        }

        private void GetLocationData(Xamarin.Essentials.Location pLocation)
        {
            mArcgisMap.SetMapViewMapPosition(pLocation.Latitude, pLocation.Longitude, 5000);
            mLastLocation = pLocation;
        }

        private void GetGPSReRoutedAsync()
        {
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(async () =>
            {
                GetReRoutedData(await Xamarin.Essentials.Geolocation.GetLocationAsync(mRequest));
            });
        }

        private void GetReRoutedData(Xamarin.Essentials.Location pLocation)
        {
            mArcgisMap.RoutedAddress(mArcgisMap.GetWgs84MapPoint(36.392415966496, 127.31265386872), mArcgisMap.GetWgs84MapPoint(pLocation.Latitude, pLocation.Longitude), RoutedCatchCallBack);
        }

        private void RoutedCatchCallBack(string obj)
        {
            Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (obj == string.Empty)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "아직 위치를 잡지 못함", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Error", obj, "OK");
                }
            });
        }

        private void NavigateReRoutedCallBack(int obj)
        {
            if(obj == -2)
            {
                /*
                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                {
                    xReRoutedTimeLabel.Text = string.Empty;
                });
                */
                xBusyIndicator.IsRunning = true;
            }
            else if(obj == -1)
            {
                /*
                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                {
                    xReRoutedTimeLabel.Text = string.Empty;
                });
                */
                xBusyIndicator.IsRunning = false;
            }
            else if(obj == 100)
            {
                /*
                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                {
                    xReRoutedTimeLabel.Text = string.Empty;
                });
                */
                GetGPSReRoutedAsync();
            }
            else
            {
                /*
                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
                {
                    xReRoutedTimeLabel.Text = "time limit : " + (15 - obj).ToString() + "Second";
                });
                */
            }
        }
    }
}
