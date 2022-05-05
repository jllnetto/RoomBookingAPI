using Business.Interfaces.Repositories.Base;
using Business.Models;

namespace Business.Interfaces.Repositories
{
    public interface IRoomRepository : IRepository<Room>
    {
        public Task<bool> CheckAvailability(Guid? id, DateTime dateStart, DateTime dateEnd);
    }
}
