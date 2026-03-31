using OTC.Api.Models;

namespace OTC.Api.Services;

public interface IScheduleService
{
    Task<IEnumerable<ScheduleListItem>> GetScheduleListAsync(string fromDate, string toDate, string username, string? searchField = null, string? startWith = null);
    Task<bool> InsertScheduleAsync(ScheduleInsertRequest request);
    Task<bool> UpdateScheduleAsync(ScheduleUpdateRequest request);
    Task<bool> DeleteScheduleAsync(string scheduleId, string username);
    Task<IEnumerable<ActivityType>> GetActivityTypesAsync();
}
