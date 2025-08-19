using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdTask.Auth.Domain.Entities;

namespace ThirdTask.Auth.Persistence.Context
{
    public class authContext : IdentityDbContext<AppUser,AppRole,int>
    {
        public authContext(DbContextOptions<authContext> options) : base(options)
        {

        }
    }
}
