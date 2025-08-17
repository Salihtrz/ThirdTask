using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdTask.Auth.Application.Interfaces;
using ThirdTask.Auth.Domain.Entities;

namespace ThirdTask.Auth.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<AppRole> _roleManager;

        public RoleService(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task CreateRoles(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync("Writer"))
            {
                await _roleManager.CreateAsync(new AppRole
                {
                    Name = "Writer"
                });
            }
            if(!await _roleManager.RoleExistsAsync("Reader"))
            {
                await _roleManager.CreateAsync(new AppRole
                {
                    Name = "Reader"
                });
            }
        }
    }
}
