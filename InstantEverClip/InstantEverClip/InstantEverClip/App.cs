
using Xamarin.Forms;

namespace me.u6k.InstantEverClip
{
    public class App : Application
    {
        public App()
        {
            MainPage = new MainPage();
        }
    }

    class MainPage : ContentPage
    {
        public MainPage()
        {
            Button btn = new Button
            {
                Text = "アラートを表示"
            };
            btn.Clicked += (s, a) =>
            {
                DisplayAlert("アラート", "動作確認", "OK");
            };

            Content = btn;
        }
    }
}
