using System;
using Xamarin.Forms;
using Plugin.Toasts;

namespace me.u6k.InstantEverClip
{
    public class App : Application
    {
        public App()
        {
            MainPage = new MainPage();
        }

        public async static void ShowToast(ToastNotificationType type, string title, string message)
        {
            var notificator = DependencyService.Get<IToastNotificator>();
            bool tapped = await notificator.Notify(type, title, message, TimeSpan.FromSeconds(2));
        }
    }

    class MainPage : ContentPage
    {
        public MainPage()
        {
            Button btn = new Button
            {
                Text = "トーストを表示"
            };
            btn.Clicked += (s, e) => App.ShowToast(ToastNotificationType.Info, "タイトル", "メッセージ");

            Content = btn;
        }
    }
}
