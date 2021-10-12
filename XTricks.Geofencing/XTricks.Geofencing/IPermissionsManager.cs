using System.Threading.Tasks;
using XTricks.Geofencing.Droid;

namespace XTricks.Geofencing
{
    public interface IPermissionsManager
    {
        PermissionCheckResult Check();
    }
}
