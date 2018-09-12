namespace ESFA.DC.EAS1819.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using ESFA.DC.EAS1819.Data;
    using ESFA.DC.EAS1819.Interface;
    using Microsoft.EntityFrameworkCore;

    public class Repository<T> : IRepository<T>
        where T : BaseEntity
    {
        private readonly EasdbContext _context;
        private readonly DbSet<T> entities;
        string _errorMessage = string.Empty;

        public Repository(EasdbContext context)
        {
            _context = context;
            entities = context.Set<T>();
        }

        #region Properties
        /// <summary>
        /// Gets a table
        /// </summary>
        public virtual IQueryable<T> Table => entities;

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        public virtual IQueryable<T> TableNoTracking => entities.AsNoTracking();

        #endregion

        #region methods

        public IEnumerable<T> GetAll()
        {
            return entities.AsQueryable();
        }

        public virtual IQueryable<T> AllIncluding(
            params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }

            return query;
        }

        public void Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            entities.Add(entity);
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            _context.SaveChanges();
        }

        public void Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            entities.Remove(entity);
            _context.SaveChanges();
        }

        #endregion
    }
}
