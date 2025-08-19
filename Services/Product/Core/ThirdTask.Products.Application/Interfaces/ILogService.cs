using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThirdTask.Products.Application.Interfaces
{
    public interface ILogService
    {
        Task LogAsync(string serviceName, string level, string message);
        Task LogAsyncExecute(string serviceName, Exception ex, bool isCritical = false);
    }
}
