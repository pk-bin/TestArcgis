using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TestArcgis
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();
        }

        protected override void OnStart()
        {
            Esri.ArcGISRuntime.ArcGISRuntimeEnvironment.ApiKey = "AAPK9514ceeaeccf4598a2b081957ccb60613pEYB8iJsrE_sgo9iS0a1P5TN3lZCn78Nrn8PO-SEFlEJ-3SxgBjkBZSodMW7CKf";
        }

        protected override void OnSleep()
        {
            System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
        }

        protected override void OnResume()
        {
        }
    }
}
