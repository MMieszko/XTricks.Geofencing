using System;
using XTricks.Shared.Contracts;

namespace XTricks.Geofencing
{
    public class LocationLog : ILocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Accuracy { get; set; }
        public DateTime DateTime { get; set; }

        public LocationLog()
        {

        }

        public LocationLog(double lat, double lng, double acc, DateTime date)
        {
            this.Latitude = lat;
            this.Longitude = lng;
            this.Accuracy = acc;
            this.DateTime = date;
        }

        public LocationLog(double lat, double lng, double acc)
           : this(lat, lng, acc, default)
        {
        }

        public LocationLog(double lat, double lng)
           : this(lat, lng, default, default)
        {

        }
    }
}
