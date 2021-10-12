using System;

namespace XTricks.Geofencing
{
    public class StartResult
    {
        public bool Succeeded { get; private set; }
        public StartFailureType Type { get; private set; }
        public string Message { get; private set; }
        public Exception Exception { get; private set; }

        private StartResult() { }

        public static StartResult CreateSucceeded() => new StartResult { Succeeded = true };
        public static StartResult CreateFailed(StartFailureType type, Exception exception, string message = null)
        {
            return new StartResult
            {
                Type = type,
                Message = string.IsNullOrEmpty(message) ? exception?.ToString() : message,
                Exception = exception,
                Succeeded = false
            };
        }
        public static implicit operator bool(StartResult value)
        {
            return value.Succeeded;
        }
    }
}
