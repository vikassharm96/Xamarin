using System;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Arch.Lifecycle;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;

namespace demo
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        MainActivityViewModel ViewModel;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Initialize();
        }

        public void Initialize()
        {
            ViewModel = ViewModelProviders.(this).Get(Java.Lang.Class.FromType(typeof(MainActivityViewModel))) as MainActivityViewModel;

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            Button SaveText = FindViewById<Button>(Resource.Id.btn_save_text);
            SaveText.Click += SaveOnClick;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_share)
            {
                _ = ShareFile();
                _ = ReadCountAsync();
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private async Task ShareFile()
        {
            //var backingFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "demo.txt");
            var backingFile = Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "demo.txt");
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Select",
                File = new ShareFile(Path.GetFileName(backingFile))
            });
        }

        private void SaveOnClick(object sender, EventArgs eventArgs)
        {
            _ = SaveTextAsync("vikas sharma");
            View view = (View)sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public async Task SaveTextAsync(string text)
        {
            var backingFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "demo.txt");
            var writer = File.CreateText(backingFile);
            await writer.WriteLineAsync(text);
        }

        public async Task<string> ReadCountAsync()
        {
            var backingFile = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "demo.txt");

            if (backingFile == null || !File.Exists(backingFile))
            {
                return "";
            }

            var text = "";
            using (var reader = new StreamReader(backingFile, true))
            {
                string line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    text = line;
                }
            }

            return text;
        }
    }
}

