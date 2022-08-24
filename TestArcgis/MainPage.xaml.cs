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
        public MainPage()
        {
            InitializeComponent();
            mArcgisMap = new Common.EsriMapComponent();
            xGrid.Children.Add(mArcgisMap.GetMapView());
            mArcgisMap.CreateMap();

            this.Appearing += MainPage_Appearing;
        }

        private void MainPage_Appearing(object sender, EventArgs e)
        {
        }
    }
}
