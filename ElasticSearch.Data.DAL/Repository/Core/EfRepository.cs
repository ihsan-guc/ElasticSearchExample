using ElasticSearchExample.Entites.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ElasticSearchExample.Data.DAL.Repository.Core
{
    public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity<int>
    {
        protected PersonContext _context;
        protected DbSet<TEntity> _dbSet;
        protected DbSet<TEntity> DbSet
        {
            get { return _dbSet; }
            set { _dbSet = value; }
        }
        public EfRepository(PersonContext context)
        {
            _dbSet = context.Set<TEntity>();
            _context = context;
        }
        public IQueryable<TEntity> GetQueryable()
        {
            return DbSet as IQueryable<TEntity>;
        }
        public IEnumerable<TEntity> GetAll()
        {
            return DbSet.ToList();
        }
        public void Delete(TEntity entity)
        {
            DbSet.Remove(entity);
        }
        public void Add(TEntity entity)
        {
            DbSet.Add(entity);
        }
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
