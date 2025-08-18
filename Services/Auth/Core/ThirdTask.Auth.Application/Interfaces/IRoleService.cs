using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThirdTask.Auth.Application.Interfaces
{
    public interface IRoleService
    {
        Task CreateRoles(string roleName);
    }
}
