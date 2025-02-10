using System.Threading.Tasks;

namespace ContactKeeper.Contracts
{
    public interface IunitOfWork
    {
        IuserRepository UserRepository { get; }
        void BeginTransaction();
        Task CommitAsync();
        void Rollback();
        void Dispose();
    }
}