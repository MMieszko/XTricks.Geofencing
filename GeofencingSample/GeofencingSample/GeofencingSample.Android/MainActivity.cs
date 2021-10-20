using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;

using AndroidX.Core.App;
using Plugin.Permissions;
using XTricks.Geofencing.Android;

namespace GeofencingSample.Droid
{
    [Activity(Label = "GeofencingSample", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            GeofencingInitializer.Initialize(builder =>
            {
                return builder
                       .WithInterval(TimeSpan.FromSeconds(15))
                       .WithPriority(Android.Gms.Location.LocationRequest.PriorityHighAccuracy)
                       .WithNotification(CreateNotification())
                       .Build();
            });

            LoadApplication(new App());
        }

        private Notification CreateNotification()
        {
            var name = "MyChannel";
            var description = "Geofencing channel";
            var importance = NotificationImportance.High;

            NotificationChannel channel = new NotificationChannel("MyChannelId", name, importance)
            {
                Description = description
            };

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);

            NotificationCompat.Builder builder = new NotificationCompat.Builder(this, channel.Id)
                                            .SetContentTitle("Geofencing is running")
                                            .SetContentText("Much longer text that cannot fit one line...")
                                            .SetStyle(new NotificationCompat.BigTextStyle()
                                                    .BigText("Much longer text that cannot fit one line..."))
                                            .SetPriority((int)NotificationPriority.High);
            return builder.Build();

        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}