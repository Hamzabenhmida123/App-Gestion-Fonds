using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FondsSocial.Infrastructure.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity); // soft delete handled by services/unit of work

        /// <summary>
        /// Filtre et pagine côté SQL (WHERE + COUNT + OrderBy + Skip/Take exécutés par la base),
        /// plutôt que de charger toute la table puis filtrer/paginer en mémoire.
        /// </summary>
        Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
            int page,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);
    }
}
