using Business.Models.Base;
using Business.Utils.Domain.Utils;
using System.Linq.Expressions;

namespace Business.Interfaces.Repositories.Base
{
    public interface IRepository<TEntity> : IDisposable where TEntity : Entity
    {
        Task<TEntity> Add(TEntity entity);
        Task<TEntity> GetByIdNoTracking(Guid id);
        Task<TEntity> GetById(Guid id);
        Task<List<TEntity>> GetAll();
        Task<TEntity> Update(TEntity entity);
        Task Remove(Guid id);
        Task<Paginator<TEntity>> Search(Expression<Func<TEntity, bool>> predicate, int currentPage = 1, int itemsPerPage = 30);
        Task<int> SaveChanges();
    }
}
