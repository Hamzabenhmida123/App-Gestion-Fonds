using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FondsSocial.Domain.Common;

namespace FondsSocial.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly FondsSocial.Infrastructure.Data.FondsSocialDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(FondsSocial.Infrastructure.Data.FondsSocialDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual void Delete(T entity)
        {
            // soft delete
            entity.IsDeleted = true;
            Update(entity);
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Id == id);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual async Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
        {
            IQueryable<T> query = _dbSet;
            if (filter != null) query = query.Where(filter);

            var totalCount = await query.CountAsync();

            query = orderBy != null ? orderBy(query) : query.OrderBy(e => e.Id);
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, totalCount);
        }
    }
}
