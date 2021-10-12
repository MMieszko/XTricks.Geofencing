namespace XTricks.Geofencing.CrossInterfaces
{
    public interface ILocationProvider
    {
        StartResult Start();
        void Stop();
    }
}
