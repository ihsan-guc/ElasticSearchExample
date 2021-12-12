using ElasticSearchExample.Entites.Entities;
using System.Collections.Generic;
using System.Linq;

namespace ElasticSearchExample.Data.DAL.Repository.Core
{
    public interface IRepository<TEntity>
        where TEntity : IEntity<int>
    {
        IQueryable<TEntity> GetQueryable();
        IEnumerable<TEntity> GetAll();
        void Delete(TEntity entity);
        void Add(TEntity entity);
        void SaveChanges();
    }
}
