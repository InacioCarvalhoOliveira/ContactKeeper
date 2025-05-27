using ContactKeeper.Contracts;
using ContactKeeper.Data;
using ContactKeeper.Models;
using Microsoft.EntityFrameworkCore;

namespace ContactKeeper.Ineterfaces
{
    public class UserContactRepository : IUserContactRepository
    {
        private readonly DataContext _context;

        public UserContactRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<UserContact> AddUserContact(UserContact userContact)
        {
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userContact.UserId);

            if (user == null)
            {
                throw new Exception("cant add user contact, user not found");
            }

            _context.UserContacts.Add(userContact);
            await _context.SaveChangesAsync();

            return userContact;
        }

        public async Task<UserContact> DeleteUserContact(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserContact> GetUserContactById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<UserContact>> GetUserContact()
        {
            var userContacts = await _context.UserContacts
                .AsNoTracking()
                .ToListAsync();

            return userContacts;
        }

        public async Task<UserContact> UpdateUserContact(UserContact userContact)
        {
            throw new NotImplementedException();
        }
    }
}