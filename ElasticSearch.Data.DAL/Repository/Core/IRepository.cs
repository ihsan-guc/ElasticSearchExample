using ElasticSearchExample.Entites.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ElasticSearchExample.Data.DAL.Repository.Core
{
    public interface IRepository<TEntity>
        where TEntity : IEntity<int>
    {
        IQueryable<TEntity> GetQueryable();
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> where);
        TEntity GetSingle(int id);
        TEntity GetById(int id);
        TModel GetById<TModel>(Func<TEntity, TModel> selector, int id);
        void Delete(TEntity entity);
        void DeleteRange(IEnumerable<TEntity> entities);
        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Attach(TEntity entity);
        void Detach(TEntity entity);
        void SaveChanges();
    }
}
