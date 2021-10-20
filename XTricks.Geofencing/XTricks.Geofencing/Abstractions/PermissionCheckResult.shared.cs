namespace XTricks.Geofencing.Abstractions
{
    public class PermissionCheckResult
    {
        public bool Succeeded { get; private set; }
        public string MissingPermission { get; private set; }

        private PermissionCheckResult()
        {

        }

        public static implicit operator bool(PermissionCheckResult value)
        {
            return value.Succeeded;
        }

        public static PermissionCheckResult CreateSucceeded() => new PermissionCheckResult { Succeeded = true };
        public static PermissionCheckResult CreateFailed(string missing) => new PermissionCheckResult { MissingPermission = missing };
    }
}