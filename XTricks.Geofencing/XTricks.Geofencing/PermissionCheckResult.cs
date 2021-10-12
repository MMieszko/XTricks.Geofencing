namespace XTricks.Geofencing.Droid
{
    public class PermissionCheckResult
    {
        public bool Suceeded { get; private set; }
        public string MissingPermission { get; private set; }

        private PermissionCheckResult()
        {

        }

        public static implicit operator bool(PermissionCheckResult value)
        {
            return value.Suceeded;
        }

        public static PermissionCheckResult CreateSucceeded() => new PermissionCheckResult { Suceeded = true };
        public static PermissionCheckResult CreateFailed(string missing) => new PermissionCheckResult { MissingPermission = missing };
    }
}