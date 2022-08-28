using System;
using System.Collections.Generic;
using TestArcgis.Model;
using Xamarin.Forms;

namespace TestArcgis.shellpage
{
    public partial class TestPage2 : ContentPage
    {
        Common.EsriMapComponent mArcgisMap;
        private Xamarin.Essentials.GeolocationRequest mRequest = null;
        private Model.VworldPlacePoi.Point mEndPoint = null;

        public TestPage2()
        {
            InitializeComponent();

            mArcgisMap = new Common.EsriMapComponent(true);
            xGrid.Children.Add(mArcgisMap.GetMapView());
            mArcgisMap.CreateMap();
            mArcgisMap.GetReRoutedCallBack(ReRoutedCallBackFunction);

            mRequest = new Xamarin.Essentials.GeolocationRequest(Xamarin.Essentials.GeolocationAccuracy.Best, TimeSpan.FromSeconds(5));
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
            if (mEndPoint != null)
            {
                mArcgisMap.RoutedAddress(mArcgisMap.GetWgs84MapPoint(Convert.ToDouble(mEndPoint.y), Convert.ToDouble(mEndPoint.x)), mArcgisMap.GetWgs84MapPoint(pLocation.Latitude, pLocation.Longitude), RoutedCatchCallBack);
            }
        }

        private void RoutedCatchCallBack(string obj)
        {

        }

        private void ReRoutedCallBackFunction(int obj)
        {
            if (obj == -2)
            {
                xBusyIndicator.IsRunning = true;
            }
            else if (obj == -1)
            {
                xBusyIndicator.IsRunning = false;
            }
            else if (obj == 100)
            {
                GetGPSReRoutedAsync();
            }
            else
            {

            }
        }

        protected override void OnAppearing()
        {
            mArcgisMap.InitAppearing();
        }

        protected override void OnDisappearing()
        {
            mArcgisMap.InitDisAppearing();
        }
        protected override void OnParentSet()
        {
            
        }

        private void xRoutedButton_Clicked(System.Object sender, System.EventArgs e)
        {
            if (mEndPoint == null)
            {
                return;
            }
            else
            {
                GetGPSReRoutedAsync();
            }

        }

        private void SearchButton_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(async () =>
                {
                    if (xDestinationPathLabel.Text == string.Empty)
                    {
                        mEndPoint = null;
                        return;
                    }
                    string _iputtext = xDestinationPathLabel.Text.Trim();
                    await Navigation.PushAsync(new PlaceSearchPage(_iputtext, GetDestinationPoiItem));
                });
            }
            catch
            {
                mEndPoint = null;
            }
        }

        private void GetDestinationPoiItem(VworldPlacePoi.PoiItem pTarget)
        {
            if (pTarget != null)
            {
                mEndPoint = pTarget.point;
            }
            else
            {
                mEndPoint = null;
            }
        }
    }
}
