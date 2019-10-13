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
using demo.ViewModels;
using Xamarin.Essentials;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace demo.Views
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar")]
    public class MainActivity : AppCompatActivity
    {
        MainActivityViewModel ViewModel;
        string fileLocation;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Initialize();
        }

        public void Initialize()
        {
            ViewModel = ViewModelProviders.Of(this).Get(Java.Lang.Class.FromType(typeof(MainActivityViewModel))) as MainActivityViewModel;

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            Button SaveText = FindViewById<Button>(Resource.Id.btn_save_text);
            SaveText.Click += SaveOnClick;

            //fileLocation = Path.Combine(Xamarin.Essentials.FileSystem.AppDataDirectory, "demo.txt");
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
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private async Task ShareFile()
        {
            // saving file to internal now to share operation simply do it by copy
            // to external storage and share
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Select",
                File = new ShareFile(Path.GetFileName(fileLocation))
            });
        }

        private async void SaveOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            EditText editText = new EditText(this);
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            var fileText = await Task.Run(() => ViewModel.ReadTextAsync());
            editText.Text = fileText;
            builder.SetTitle("Save Text");
            builder.SetView(editText);
            builder.SetPositiveButton("Save", (s, args) => {
                if (String.IsNullOrWhiteSpace(editText.Text))
                {
                    Snackbar.Make(view, "Please enter input!", Snackbar.LengthLong)
                    .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
                    return;
                }
                else
                {
                    _ = ViewModel.SaveTextAsync(editText.Text);
                    if (true)
                    {
                        Snackbar.Make(view, "Data saved successfully!", Snackbar.LengthLong)
                        .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
                        return;
                    }
                }
            });
            builder.Show();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }
}
