namespace ESFA.DC.EAS1819.DataService.Interface
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using ESFA.DC.EAS1819.EF;

    public interface IRepository<T>
        where T : BaseEntity
    {
        #region Properties

        /// <summary>
        /// Gets a table
        /// </summary>
        IQueryable<T> Table { get; }

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        IQueryable<T> TableNoTracking { get; }

        #endregion

        #region Methods

        IEnumerable<T> GetAll();

        IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);

        void Insert(T entity);

        void Update(T entity);

        void Delete(T entity);
        #endregion
    }
}
