using System.Drawing;
using ContactKeeper.Contracts;
using ContactKeeper.Data;
using ContactKeeper.Models;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace ContactKeeper.Interfaces
{
    public sealed class UnitOfWork : DbSession, IunitOfWork
    {
        private readonly DataContext _context;
        private readonly IuserRepository _userRepository;
        private readonly IUserContactRepository _userContactRepository;
        private IDbContextTransaction _transaction;

       public  UnitOfWork(IOptions<DatabaseSettings> 
        databaseSettings,
        DataContext context,
        IuserRepository userRepository,
        IUserContactRepository userContactRepository) : base(databaseSettings)
        {
            _context = context;
            _userRepository = userRepository;
            _userContactRepository = userContactRepository;
        }

        public new void BeginTransaction()
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

        public new void Rollback()
        {
            _transaction?.Rollback();
        }

        public IuserRepository UserRepository => _userRepository;

        public IUserContactRepository UserContactRepository => _userContactRepository;

        public new void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }

    }

}