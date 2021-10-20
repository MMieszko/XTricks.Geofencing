namespace XTricks.Geofencing.Abstractions
{
    public interface ILocationProvider
    {
        StartResult Start();
        void Stop();
    }
}
