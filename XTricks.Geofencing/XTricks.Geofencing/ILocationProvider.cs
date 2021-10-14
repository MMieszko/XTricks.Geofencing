namespace XTricks.Geofencing
{
    public interface ILocationProvider
    {
        StartResult Start();
        void Stop();
    }
}
