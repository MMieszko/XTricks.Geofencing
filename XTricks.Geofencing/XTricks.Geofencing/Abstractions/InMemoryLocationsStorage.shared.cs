using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XTricks.Shared.Collections;

namespace XTricks.Geofencing.Abstractions
{
    public class InMemoryLocationsStorage : ILocationLogsStorage
    {
        private readonly SinkStack<LocationLog> _logs;

        public InMemoryLocationsStorage(int maximumCapacity)
        {
            if (maximumCapacity <= 1)
                throw new ArgumentException("Capacity cannot be less or equal 1");

            _logs = new SinkStack<LocationLog>(maximumCapacity);
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
