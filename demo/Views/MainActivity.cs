using System;
using System.IO;
using System.Threading.Tasks;
using Android.App;
using Android.Arch.Lifecycle;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Support.V4.Content ;
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
        readonly string[] Permissions =
        {
            Android.Manifest.Permission.ReadExternalStorage,
            Android.Manifest.Permission.WriteExternalStorage
        };
        const int STORAGE_REQUEST_CODE = 101;

        MainActivityViewModel ViewModel;
        bool PermissionDenied = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            ExperimentalFeatures.Enable(ExperimentalFeatures.EmailAttachments, ExperimentalFeatures.ShareFileRequest);
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

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;
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
            if (id == Resource.Id.action_share_text)
            {
                _ = ShareText();
                return true;
            } else if(id == Resource.Id.action_share)
            {
                _ = ShareFile();
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private async Task ShareText()
        {
            var fileText = await Task.Run(() => ViewModel.ReadTextAsync());
            await Share.RequestAsync(new ShareTextRequest
            {
                Title = "Share Text",
                Text = fileText
            });
            
        }

        private async Task ShareFile()
        {
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Share File",
                File = new ShareFile(ViewModel.InternalFileLocation)
            });
        }

        private async void SaveOnClick(object sender, EventArgs eventArgs)
        {
            if (RequestStoragePermission())
            {
                View view = (View)sender;
                EditText editText = new EditText(this);
                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                var fileText = await Task.Run(() => ViewModel.ReadTextAsync());
                editText.Text = fileText;
                builder.SetTitle("Save Text");
                builder.SetView(editText);
                builder.SetPositiveButton("Save", (s, args) =>
                {
                    if (String.IsNullOrWhiteSpace(editText.Text))
                    {
                        Snackbar.Make(view, Resource.String.enter_input, Snackbar.LengthShort)
                        .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
                        return;
                    }
                    else
                    {
                        _ = ViewModel.SaveTextAsync(editText.Text);
                        Snackbar.Make(view, Resource.String.data_saved, Snackbar.LengthShort)
                        .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
                    }
                });
                builder.Show();
            }
        }

        private bool RequestStoragePermission()
        {
            if(ContextCompat.CheckSelfPermission(this, Permissions[0]) == Permission.Granted)
            {
                return true;
            } else if(ShouldShowRequestPermissionRationale(Permissions[0]))
            {
                var permissionExplanation = new AlertDialog.Builder(this)
                .SetMessage(Resource.String.storage_permission)
                .SetCancelable(false)
                .SetPositiveButton(Resource.String.allow_storage, (sender, args) =>
                {
                    RequestPermissions(Permissions, STORAGE_REQUEST_CODE);
                })
                .Create();
                permissionExplanation.Show();
            } else
            {
                if (PermissionDenied)
                {
                    var goToSetting = new AlertDialog.Builder(this)
                    .SetMessage(Resource.String.setting_page)
                    .SetCancelable(false)
                    .SetPositiveButton(Resource.String.allow, (sender, args) =>
                    {
                        var intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings, Android.Net.Uri.FromParts("package", this.PackageName, null));
                        intent.AddFlags(ActivityFlags.NewTask);
                        StartActivity(intent);
                    })
                    .Create();
                    goToSetting.Show();
                } else
                {
                    RequestPermissions(Permissions, STORAGE_REQUEST_CODE);
                }
            }
            return false;
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            if (GetVersion().Equals("1.0"))
            {
                View view = (View)sender;
                Snackbar.Make(view, Resource.String.updated_app, Snackbar.LengthLong)
                    .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
            } else
            {
                StartActivity(new Intent(Application.Context, typeof(DetailActivity)));
            }
            
        }

        private string GetVersion()
        {
            Version version = AppInfo.Version;
            return string.Format("{0}.{1}", version.Major, version.Minor);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            switch(requestCode)
            {
                case STORAGE_REQUEST_CODE:
                    if(permissions.Length > 0 && grantResults[0] == Android.Content.PM.Permission.Granted)
                    {

                    } else
                    {
                        if (ShouldShowRequestPermissionRationale(Permissions[0]))
                        {
                            RequestPermissions(Permissions, STORAGE_REQUEST_CODE);
                        } else
                        {
                            PermissionDenied = true;
                        }
                    }
                    break;
            }
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
