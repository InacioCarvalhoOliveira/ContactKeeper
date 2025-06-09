using ContactKeeper.Services.Repositories;

namespace ContactKeeper.Services.Repositories
{
    public interface IunitOfWork
    {
        //IuserRepository UserRepository { get; }
        void BeginTransaction();
        Task CommitAsync();
        void Rollback();
        void Dispose();
    }
}