using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XTricks.Shared.Collections;

namespace XTricks.Geofencing.Storage
{
    public class InMemoryLocationsStorage : ILocationLogsStorage
    {
        private readonly SinkStack<LocationLog> _logs;

        public InMemoryLocationsStorage()
        {
            _logs = new SinkStack<LocationLog>(20);
        }

        public Task AddAsync(LocationLog location)
        {
            _logs.Push(location);

            return Task.CompletedTask;
        }

        public Task<List<LocationLog>> GetAsync()
        {
            var result = _logs.Where(x => !(x is null)).ToList();

            return Task.FromResult(result);
        }
    }
}
