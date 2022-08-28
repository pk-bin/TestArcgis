using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace TestArcgis.shellpage
{
    public partial class TestPage1 : ContentPage
    {
        Common.EsriMapComponent mArcgisMap;
        public TestPage1()
        {
            InitializeComponent();

            mArcgisMap = new Common.EsriMapComponent(false);
            xGrid.Children.Add(mArcgisMap.GetMapView());
            mArcgisMap.CreateMap();
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
    }
}
