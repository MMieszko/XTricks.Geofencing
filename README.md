# XTricks.Geofencing

The library which gives quick and easy ability to track whenever user entered or exited given location. The geofencing is working in aswell background and foregourd
You can download the library from [NuGet](https://www.nuget.org/packages/XTricks.Geofencing/0.1.9)


# Android
The application is using [WakeLock](https://developer.android.com/training/scheduling/wakelock) feature to keep device awake when app goes into 
background. Keep in mind to use it only when nessesary 

Make sure to add following permissions into your Manifest file

```` 
```
	<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
	<uses-permission android:name="android.permission.ACCESS_BACKGROUND_LOCATION" />
	<uses-permission android:name="android.permission.WAKE_LOCK" />
	<uses-permission android:name="android.permission.FOREGROUND_SERVICE" />
	<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
```
````

Inside your MainAcitivty.cs class add following code in OnCreate() method

```` 
GeofencingInitializer.Initialize(builder =>
            {
                return builder
                       .WithInterval(TimeSpan.FromSeconds(15))
                       .WithPriority(Android.Gms.Location.LocationRequest.PriorityHighAccuracy)
                       .WithNotification(CreateNotification())
                       .Build();
            });
````

In above example you have builder which initializes the geolocation gathering by Android aswell as Notification to display.
The Notification is mandatory in order to use Android service. Here is an example of creating Notification in Android:


```` 
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
````

# iOS

Not implemented yet


# Usage
Feel free to check the Sample application inside the repositroy. The most basic usage would be as below.
Firstly lets get some location which we want to track. For example lets monitor geofencing for
Playa de Roque Burmejo (28.5795767,-16.136934). To add the location lets use our Geofencing Service.


```` 
```
GeofencingService.Instance.LocationDetected += GeofenceFired;

var playa = new MonitoredLocation("PlayaDeRoqueBurmejo", 28.5795767, -16.136934, Distance.FromMeters(100), Distance.FromMeters(200), GeofenceDirection.Enter | GeofenceDirection.Exit);

GeofencingService.Instance.AddLocation(playa);

GeofencingService.Instance.Start();

...

private void GeofenceFired(object sender, LocationDetectedEventArgs e)
{
    Debug.Write($"We {(e.Direction == GeofenceDirection.Exit ? "exited" : "entered")} into {e.Location.Key}");
}

```` 
```

In above example we created MonitoredLocation object which we would like to track. The first parameter is an key with the object type. This must
be unique value which identifies your location. Geofencing enter will be fired whenever user is in range of 100 meters from given location or will be far away for more than 200 meters.








