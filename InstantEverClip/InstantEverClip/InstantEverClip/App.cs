using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using Xamarin.Forms;
using Plugin.Toasts;
using Thrift.Transport;
using Thrift.Protocol;
using Evernote.EDAM.Type;
using Evernote.EDAM.UserStore;
using Evernote.EDAM.NoteStore;
using HtmlAgilityPack;

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
            await notificator.Notify(type, title, message, TimeSpan.FromSeconds(2));
        }
    }

    class MainPage : ContentPage
    {
        private Entry urlEntry;

        public MainPage()
        {
            Label urlEntryLabel = new Label
            {
                Text = "URL"
            };
            urlEntry = new Entry
            {
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);

            StackLayout layout1 = new StackLayout
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                Orientation = StackOrientation.Horizontal,
                Children = { urlEntryLabel, urlEntry }
            };

            Button btn = new Button
            {
                Text = "ノートを作成"
            };
            btn.Clicked += ClickedCreateNoteButton;

            StackLayout rootLayout = new StackLayout
            {
                Children = { layout1, btn }
            };

            Content = rootLayout;
        }

        private async void ClickedCreateNoteButton(object sender, EventArgs e)
        {
            try
            {
                string authToken = "authToken";
                string evernoteHost = "sandbox.evernote.com";

                string url = urlEntry.Text;
                if (string.IsNullOrWhiteSpace(url))
                {
                    throw new Exception("URLを入力してください。");
                }
                Debug.WriteLine("URL=" + url);

                Uri userStoreUrl = new Uri("https://" + evernoteHost + "/edam/user");
                TTransport userStoreTransport = new THttpClient(userStoreUrl);
                TProtocol userStoreProtocol = new TBinaryProtocol(userStoreTransport);
                UserStore.Client userStore = new UserStore.Client(userStoreProtocol);

                bool versionOk = userStore.checkVersion("InstantEverClip",
                    Evernote.EDAM.UserStore.Constants.EDAM_VERSION_MAJOR,
                    Evernote.EDAM.UserStore.Constants.EDAM_VERSION_MINOR);
                if (!versionOk)
                {
                    throw new Exception("バージョン・チェックがエラーになりました。");
                }

                var htmlDoc = new HtmlDocument();
                var req = WebRequest.Create(url);
                var res = await req.GetResponseAsync();
                using (Stream s = res.GetResponseStream())
                {
                    var buf = new byte[1024 * 1024];
                    var ms = new MemoryStream();
                    var len = 0;
                    while ((len = s.Read(buf, 0, buf.Length)) > 0)
                    {
                        ms.Write(buf, 0, len);
                    }

                    buf = ms.ToArray();
                    var html = "";
                    var enc = Hnx8.ReadJEnc.ReadJEnc.JP.GetEncoding(buf, buf.Length, out html);
                    Debug.WriteLine("HTML Encoding=" + enc.ToString());
                    htmlDoc.LoadHtml(html);
                }
                HtmlNode titleNode = htmlDoc.DocumentNode.Descendants("title").First();
                String noteTitle = titleNode.InnerText;
                if (noteTitle.Trim().Length == 0)
                {
                    noteTitle = url;
                    Debug.WriteLine("title tag is not found or empty.");
                }
                else
                {
                    Debug.WriteLine("noteTitle=" + noteTitle);
                }

                string noteStoreUrl = userStore.getNoteStoreUrl(authToken);
                TTransport noteStoreTransport = new THttpClient(new Uri(noteStoreUrl));
                TProtocol noteStoreProtocol = new TBinaryProtocol(noteStoreTransport);
                NoteStore.Client noteStore = new NoteStore.Client(noteStoreProtocol);

                Note note = new Note();
                note.Title = noteTitle;
                note.Attributes = new NoteAttributes
                {
                    SourceURL = urlEntry.Text
                };
                note.Content = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                    + "<!DOCTYPE en-note SYSTEM \"http://xml.evernote.com/pub/enml2.dtd\">"
                    + "<en-note>" + noteTitle + "<br />" + urlEntry.Text + "</en-note>";

                noteStore.createNote(authToken, note);
                Debug.WriteLine("create note succeed.");
                App.ShowToast(ToastNotificationType.Success, noteTitle, "ノートを作成しました。");

                urlEntry.Text = string.Empty;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                App.ShowToast(ToastNotificationType.Error, "エラー", ex.ToString());
            }
        }
    }
}
