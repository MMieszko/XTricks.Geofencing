﻿using System;

namespace XTricks.Geofencing
{
    public class LocationChangedEventArgs : EventArgs
    {
        public LocationLog Location { get; }

        public LocationChangedEventArgs(LocationLog log)
        {
            this.Location = log;
        }
    }
}
