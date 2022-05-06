using Business.Interfaces.Repositories.Base;
using Business.Models.Base;
using Business.Utils.Domain.Utils;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data.Repositories.Base
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity, new()
    {
        protected readonly BookingDbContext Db;
        protected readonly DbSet<TEntity> DbSet;
        public Repository(BookingDbContext db)
        {
            Db = db;
            DbSet = db.Set<TEntity>();
        }

        public async Task<List<TEntity>> GetAll()
        {
            return await DbSet.ToListAsync();
        }

        public async Task<TEntity> GetByIdNoTracking(Guid id)
        {
            return await DbSet.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<TEntity> GetById(Guid id)
        {
            return await DbSet.FindAsync(id);
        }

        public async Task<Paginator<TEntity>> Search(Expression<Func<TEntity, bool>> predicate, int currentPage = 1, int itemsPerPage = 30)
        {

            IQueryable<TEntity> query = DbSet.AsNoTracking().Where(predicate);
            int count = query.Count();
            List<TEntity> data = await query
                    .OrderByDescending(c => c.CreateDate)
                    .Skip((currentPage - 1) * itemsPerPage)
                    .Take(itemsPerPage).ToListAsync();

            return new Paginator<TEntity>(data, count, currentPage, itemsPerPage);
        }

        public async Task<TEntity> Add(TEntity entity)
        {
            DbSet.Add(entity);
            await SaveChanges();
            return entity;
        }
        public async Task<TEntity> Update(TEntity entity)
        {
            DbSet.Update(entity);
            await SaveChanges();
            return entity;
        }

        public async Task Remove(Guid id)
        {
            DbSet.Remove(new TEntity { Id = id });
            await SaveChanges();
        }

        public async Task<int> SaveChanges()
        {
            return await Db.SaveChangesAsync();
        }
        public async void Dispose()
        {
            Db?.Dispose();
        }
    }
}
