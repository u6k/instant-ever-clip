using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Thrift.Transport;
using Thrift.Protocol;
using Evernote.EDAM.Type;
using Evernote.EDAM.UserStore;
using Evernote.EDAM.NoteStore;

namespace me.u6k.InstantEverClip
{
    [Activity(Label = "InstantEverClip", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            Button button = FindViewById<Button>(Resource.Id.MyButton);
            button.Click += execEvernote;
        }

        private void execEvernote(object sender, EventArgs e)
        {
            string authToken = "authToken";
            string evernoteHost = "sandbox.evernote.com";

            Uri userStoreUrl = new Uri("https://" + evernoteHost + "/edam/user");
            TTransport userStoreTransport = new THttpClient(userStoreUrl);
            TProtocol userStoreProtocol = new TBinaryProtocol(userStoreTransport);
            UserStore.Client userStore = new UserStore.Client(userStoreProtocol);

            bool versionOk = userStore.checkVersion("InstantEverClip", Evernote.EDAM.UserStore.Constants.EDAM_VERSION_MAJOR, Evernote.EDAM.UserStore.Constants.EDAM_VERSION_MINOR);
            System.Diagnostics.Trace.WriteLine("Is my Evernote API version up to date? " + versionOk);
            if (!versionOk)
            {
                Toast.MakeText(this, "Is my Evernote API version up to date? " + versionOk, ToastLength.Short).Show();
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
            System.Diagnostics.Trace.WriteLine("Successfully created new note with GUID: " + createdNote.Guid);
            Toast.MakeText(this, "Successfully created new note with GUID: " + createdNote.Guid, ToastLength.Short).Show();
        }
    }
}
