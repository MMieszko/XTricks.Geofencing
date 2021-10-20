using System;

namespace XTricks.Geofencing.Abstractions
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string msg)
            : base(msg)
        {

        }
    }
}