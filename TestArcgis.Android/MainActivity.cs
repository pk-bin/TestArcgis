using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;

namespace TestArcgis.Droid
{
    [Activity(Label = "TestArcgis", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());

            await Xamarin.Essentials.Permissions.RequestAsync<CommonPermission>();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnPostResume()
        {
            base.OnPostResume();

            _ = Xamarin.Essentials.Geolocation.GetLastKnownLocationAsync();
        }
    }

    public class CommonPermission : Xamarin.Essentials.Permissions.BasePlatformPermission
    {
        public override (string androidPermission, bool isRuntime)[] RequiredPermissions => new System.Collections.Generic.List<(string androidPermission, bool isRuntime)>
        {
            (Android.Manifest.Permission.Internet, true),
            (Android.Manifest.Permission.Bluetooth, true),
            (Android.Manifest.Permission.BluetoothAdmin, true),
            (Android.Manifest.Permission.BluetoothPrivileged, true),
            (Android.Manifest.Permission.AccessCoarseLocation, true),
            (Android.Manifest.Permission.AccessFineLocation, true),
            (Android.Manifest.Permission.AccessBackgroundLocation, true),
            (Android.Manifest.Permission.WriteExternalStorage, true),
            (Android.Manifest.Permission.ReadExternalStorage, true),
            (Android.Manifest.Permission.RequestIgnoreBatteryOptimizations, true),
        }.ToArray();
    }
}