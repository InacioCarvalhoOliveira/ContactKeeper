using System.Text.Json;
using ContactKeeper.Services.Repositories;
using ContactKeeper.Data;
using ContactKeeper.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ContactKeeper.Services.Interfaces
{
    public class UserRepository : IuserRepository
    {
        private readonly DataContext _context;
        private readonly IMemoryCache _cache;

        public UserRepository(DataContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        public async Task<List<User>> GetUsers()
        {
            var users = await _context.Users
             .AsNoTracking()
             .Include(u => u.UserContacts)
             .ToListAsync();

            return users;
        }
        public async Task<User> GetUserById(int id)
        {
            var user = await _context.Users
             .AsNoTracking()
             .Include(u => u.UserContacts)
             .FirstOrDefaultAsync(x => x.Id == id);
            return user;
        }


        public async Task<User> AddUser(User user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.Role))
                {
                    user.Role = "user";
                }

                _context.Users.Add(user);

                await _context.SaveChangesAsync();

                var newUser = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == user.Id);

                return newUser ?? user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
                return null;
            }


        }
        public async Task<UserDto> Authenticate(string username, string role, string password)
        {

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Username == username
                && x.Role == role
                && x.Password == password);
            if (user == null)
            {
                return null;
            }
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Password = user.Password,
                Role = user.Role
            };

        }      
    

    public async Task<User> DeleteUser(int id)
        {
            var userToDelete = await GetUserById(id);
            if (userToDelete != null)
            {
                _context.Users.Remove(userToDelete);
                await _context.SaveChangesAsync();
                return userToDelete;
            }
            return userToDelete;
        }

        public async Task<User> UpdateUser(User user)
        {

            var userToUpdate = await GetUserById(user.Id);
            if (userToUpdate != null)
            {
                userToUpdate.Username = user.Username;
                userToUpdate.Email = user.Email;
                userToUpdate.Password = user.Password;
                userToUpdate.Role = user.Role;
                _context.Users.Update(userToUpdate);
                await _context.SaveChangesAsync();
                return userToUpdate;
            }
            return null;
        }
    }
}