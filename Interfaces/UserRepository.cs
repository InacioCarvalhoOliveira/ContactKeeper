using System.Text.Json;
using ContactKeeper.Contracts;
using ContactKeeper.Data;
using ContactKeeper.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ContactKeeper.Interfaces
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
         
        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await _context.Users
             .AsNoTracking()
             .ToListAsync();
             
             return users;
        }
        public async Task<User> GetUserById(int id)
        {
            var user = await _context.Users
             .AsNoTracking()
             .Include(x => x.PhoneNumber)
             .FirstOrDefaultAsync(x => x.Id == id);             
             return user;
        }
        public async Task<IList<User>> GetInfoUserByDdd(int ddd)
        {
            const string cacheKey = "UsersByDdd";
            if (!_cache.TryGetValue(cacheKey, out IList<User> users))
            {
                users = await _context.Users
                .AsNoTracking()
                .Include(x => x.PhoneNumber)
                .Where(x => x.PhoneNumber.DDD == ddd)
                .ToListAsync();
                
                _cache.Set(cacheKey, users, new MemoryCacheEntryOptions{
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
                Console.WriteLine("dados do banco");
            }
            else
            {
                users = _cache.Get(cacheKey) as IList<User>;
                Console.WriteLine("dados do cache");

            }
            return users;
        }
       public async Task<User> AddUser(User user)
    {
        string path = "d:/GitHub/Dotnet/APIS/ContactKeeper/util/ddd/dddsBrasileiros.json";
        var jsonString = await File.ReadAllTextAsync(path);
        var dddsData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<string>>>>(jsonString);

        if (dddsData != null && dddsData["dddsPorEstado"].Values.Any(list => list.Contains(user.PhoneNumber.DDD.ToString())))
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        else
        {
            throw new ArgumentException("DDD inválido.");
        }
    }
        public async Task<User> DeleteUser(int id)
        {
           var userToDelete = await GetUserById(id);
              if(userToDelete != null)
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
            if(userToUpdate != null)
            {
                userToUpdate.Username = user.Username;
                userToUpdate.Email = user.Email;
                if(user.PhoneNumber != null)
                {
                    userToUpdate.PhoneNumber.DDI = user.PhoneNumber.DDI;
                    userToUpdate.PhoneNumber.DDD = user.PhoneNumber.DDD;
                    userToUpdate.PhoneNumber.LocalNumber = user.PhoneNumber.LocalNumber;
                }
                _context.Users.Update(userToUpdate);
                await _context.SaveChangesAsync();
                return userToUpdate;
            }
            return null;
        }      
    }
}