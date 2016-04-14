﻿using System;
using Xamarin.Forms;
using Plugin.Toasts;
using Thrift.Transport;
using Thrift.Protocol;
using Evernote.EDAM.Type;
using Evernote.EDAM.UserStore;
using Evernote.EDAM.NoteStore;

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
                Text = "ノートを作成"
            };
            btn.Clicked += ClickedCreateNoteButton;

            Content = btn;
        }

        private void ClickedCreateNoteButton(object sender, EventArgs e)
        {
            try
            {
                string authToken = "authToken";
                string evernoteHost = "sandbox.evernote.com";

                Uri userStoreUrl = new Uri("https://" + evernoteHost + "/edam/user");
                TTransport userStoreTransport = new THttpClient(userStoreUrl);
                TProtocol userStoreProtocol = new TBinaryProtocol(userStoreTransport);
                UserStore.Client userStore = new UserStore.Client(userStoreProtocol);

                bool versionOk = userStore.checkVersion("InstantEverClip",
                    Evernote.EDAM.UserStore.Constants.EDAM_VERSION_MAJOR,
                    Evernote.EDAM.UserStore.Constants.EDAM_VERSION_MINOR);
                if (!versionOk)
                {
                    App.ShowToast(ToastNotificationType.Error, "エラー", "バージョン・チェックがエラーになりました。");
                    return;
                }

                string noteStoreUrl = userStore.getNoteStoreUrl(authToken);
                TTransport noteStoreTransport = new THttpClient(new Uri(noteStoreUrl));
                TProtocol noteStoreProtocol = new TBinaryProtocol(noteStoreTransport);
                NoteStore.Client noteStore = new NoteStore.Client(noteStoreProtocol);

                Note note = new Note();
                note.Title = "Test";
                note.Content = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>"
                    + "<!DOCTYPE en-note SYSTEM \"http://xml.evernote.com/pub/enml2.dtd\">"
                    + "<en-note>Test " + DateTime.Now.ToString() + "</en-note>";

                Note createdNote = noteStore.createNote(authToken, note);
                App.ShowToast(ToastNotificationType.Success, "ノート作成", "ノートを作成しました。(GUID=" + createdNote.Guid + ")");
            }
            catch (Exception ex)
            {
                App.ShowToast(ToastNotificationType.Error, "エラー", ex.ToString());
            }
        }
    }
}
