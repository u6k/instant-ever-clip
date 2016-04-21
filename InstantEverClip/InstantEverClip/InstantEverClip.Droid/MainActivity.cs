using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Plugin.Toasts;
using Xamarin.Forms;

namespace me.u6k.InstantEverClip
{
    [Activity(Label = "InstantEverClip",
        Icon = "@drawable/icon",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,
        Theme = "@android:style/Theme.Holo.Light")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            DependencyService.Register<ToastNotificatorImplementation>();
            ToastNotificatorImplementation.Init(this);

            LoadApplication(new App());
        }
    }

    [Activity(Label = "InstantEverClip")]
    [IntentFilter(new[] { Intent.ActionSend },
        Categories = new[] { Intent.CategoryDefault },
        DataMimeType = "text/plain")]
    public class ClipActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            if (this.Intent.Action == Intent.ActionSend)
            {
                System.Diagnostics.Debug.WriteLine("TEXT=" + this.Intent.GetStringExtra(Intent.ExtraText));
                System.Diagnostics.Debug.WriteLine("SUBJECT=" + this.Intent.GetStringExtra(Intent.ExtraSubject));
            }
        }
    }
}