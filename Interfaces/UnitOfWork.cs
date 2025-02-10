using System.Drawing;
using ContactKeeper.Contracts;
using ContactKeeper.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace ContactKeeper.Interfaces
{
    public sealed class UnitOfWork : DbSession, IunitOfWork
    {
        private readonly DataContext _context;
        private readonly IuserRepository _userRepository;
        private IDbContextTransaction _transaction;

        public UnitOfWork(IConfiguration configuration,
         DataContext context,
         IuserRepository userRepository
         ) : base(configuration)
        {
            _context = context;
            _userRepository = userRepository;
        }

        public void BeginTransaction()
        {
            _transaction = _context.Database.BeginTransaction();
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                _transaction?.Commit();
            }
            catch
            {
                _transaction?.Rollback();
                throw;
            }
        }

        public void Rollback()
        {
            _transaction?.Rollback();
        }

        public IuserRepository UserRepository => _userRepository;

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }

    }

}