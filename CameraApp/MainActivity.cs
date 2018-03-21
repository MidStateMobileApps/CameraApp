using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Provider;
using System.Collections.Generic;
using Android.Content.PM;
using Java.IO;
using Android.Graphics;
using System;
using Android.Runtime;
using CameraApp;

namespace CameraApp
{
    public static class App
    {
        public static File _file;
        public static File _dir;
        public static Bitmap bitmap;
    }

    [Activity(Label = "CameraApp", MainLauncher = true, NoHistory = false)]
    public class MainActivity : Activity
    {
        ImageView _imageView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            if (IsThereAnAppToTakePictures())
            {
                CreateDirectoryForPictures();
                Button button = FindViewById<Button>(Resource.Id.myButton);
                _imageView = FindViewById<ImageView>(Resource.Id.imageViewCurt);

                button.Click += TakeAPic;
            }
        }

        private void TakeAPic(object sender, System.EventArgs e)
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            App._file = new Java.IO.File(App._dir, String.Format("myPhoto_{0}.jpg", Guid.NewGuid()));
            intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(App._file));
            StartActivityForResult(intent, 0);
        }

        private void CreateDirectoryForPictures()
        {
            App._dir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures);
            if (!App._dir.Exists()) App._dir.Mkdirs();
        }

        private bool IsThereAnAppToTakePictures()
        {
            Intent intent = new Intent(MediaStore.ActionImageCapture);
            IList<ResolveInfo> availableActivities = 
                PackageManager.QueryIntentActivities(intent, Android.Content.PM.PackageInfoFlags.MatchDefaultOnly);
            return availableActivities != null && availableActivities.Count > 0;
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if ( data == null && resultCode == Result.Ok)
            {
                Intent mediaIntent = new Intent(Intent.ActionMediaScannerScanFile);
                Android.Net.Uri contentUri = Android.Net.Uri.FromFile(App._file);
                mediaIntent.SetData(contentUri);
                SendBroadcast(mediaIntent);

                int height = Resources.DisplayMetrics.HeightPixels;
                int width = _imageView.Height;
                App.bitmap = App._file.Path.LoadAndResizeBitmap(width, height);

                if ( App.bitmap != null)
                {
                    _imageView.SetImageBitmap(App.bitmap);
                    App.bitmap = null;
                }
                GC.Collect();
               
            }
        }
    }
}

