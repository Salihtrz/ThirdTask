using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdTask.Products.Domain.Entities;

namespace ThirdTask.Products.Persistence.Context
{
    public class productContext : DbContext
    {
        public productContext(DbContextOptions<productContext> options):base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
    }
}
