using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ThirdTask.Products.Application.Interfaces;
using ThirdTask.Products.Persistence.Context;

namespace ThirdTask.Products.Persistence.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly productContext _productContext;

        public Repository(productContext productContext)
        {
            _productContext = productContext;
        }

        public async Task CreateAsync(T t)
        {
            _productContext.Set<T>().Add(t);
            await _productContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T t)
        {
            _productContext.Set<T>().Remove(t);
            await _productContext.SaveChangesAsync();
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _productContext.Set<T>().ToListAsync();
        }

        public async Task<T>? GetByFilterAsync(Expression<Func<T, bool>> filter)
        {
            return await _productContext.Set<T>().SingleOrDefaultAsync(filter);
        }

        public async Task<T> GetByIdAsync(int id)
        {
            return await _productContext.Set<T>().FindAsync(id);
        }

        public async Task UpdateAsync(T t)
        {
            _productContext.Set<T>().Update(t);
            await _productContext.SaveChangesAsync();
        }
    }
}
