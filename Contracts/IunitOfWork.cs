using System.Threading.Tasks;
using ContactKeeper.Models;

namespace ContactKeeper.Contracts
{
    public interface IunitOfWork
    {
        IuserRepository UserRepository { get; }
        IUserContactRepository UserContactRepository { get; }
        void BeginTransaction();
        Task CommitAsync();
        void Rollback();
        void Dispose();
    }
}