using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces
{
    public interface IBackgroundJobService
    {
        Task<string> ScheduleJobAsync(string jobId, string jobType, object data, DateTime scheduledTime);
        Task CancelJobAsync(string jobId);
    }
}
