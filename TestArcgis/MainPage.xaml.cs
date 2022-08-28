using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestArcgis
{
    public partial class MainPage : Shell
    {
        shellpage.TestPage1 mTestPage1;
        shellpage.TestPage2 mTestPage2;

        public MainPage()
        {
            InitializeComponent();
            Xamarin.Essentials.DeviceDisplay.KeepScreenOn = true;

            mTestPage1 = new shellpage.TestPage1();
            mTestPage2 = new shellpage.TestPage2();

            xTestPage1ShellContent.Content = mTestPage1;
            xTestPage2ShellContent.Content = mTestPage2;
        }
    }
}
