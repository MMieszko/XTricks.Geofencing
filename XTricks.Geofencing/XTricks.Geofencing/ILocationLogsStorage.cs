using System.Collections.Generic;
using System.Threading.Tasks;

namespace XTricks.Geofencing
{
    public interface ILocationLogsStorage
    {
        Task AddAsync(LocationLog location);
        Task<List<LocationLog>> GetAsync();
    }
}
