using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Webkit;
using Android.Support.V4.App;
using Android;
using Android.Content.PM;
using Android.Content;
using Android.Graphics;
using Android.Support.V4.Content;

namespace CarPilesApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        WebView web_view;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            if (ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != Permission.Granted ||
                ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != Permission.Granted ||
                ActivityCompat.CheckSelfPermission(this, Manifest.Permission.Internet) != Permission.Granted ||
                ActivityCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != Permission.Granted
                )
            {
                Toast.MakeText(ApplicationContext, "没有定位权限，请手动开启", ToastLength.Short).Show();
                ActivityCompat.RequestPermissions(this, new string[] {
                    Manifest.Permission.AccessCoarseLocation,
                    Manifest.Permission.AccessFineLocation,
                    Manifest.Permission.Internet,
                    Manifest.Permission.WriteExternalStorage
                }, 100);
            }
            else
            {
                web_view = FindViewById<WebView>(Resource.Id.webViewMain);
                web_view.Settings.JavaScriptEnabled = true;
                web_view.Settings.DatabaseEnabled = true;
                string dir = this.ApplicationContext.GetDir("database", FileCreationMode.Private).Path;
                web_view.Settings.SetGeolocationDatabasePath(dir);
                web_view.Settings.SetGeolocationEnabled(true);
                web_view.Settings.DomStorageEnabled = true;
                web_view.SetWebViewClient(new mywebViewClient());
                web_view.SetWebChromeClient(new myWebChromeClient());
                web_view.LoadUrl("http://43.226.150.134");
            }

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        class myWebChromeClient : WebChromeClient
        {
            public override void OnReceivedIcon(WebView view, Bitmap icon)
            {
                base.OnReceivedIcon(view, icon);
            }
            public override void OnGeolocationPermissionsShowPrompt(string origin, GeolocationPermissions.ICallback callback)
            {
                callback.Invoke(origin, true, false);
                base.OnGeolocationPermissionsShowPrompt(origin, callback);
            }
        }

        class mywebViewClient : WebViewClient
        {
            public override bool ShouldOverrideUrlLoading(WebView view, string url)
            {
                return false;
            }
        }
    }
}