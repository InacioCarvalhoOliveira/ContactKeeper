using System.Text.Json;
using ContactKeeper.Contracts;
using ContactKeeper.Data;
using ContactKeeper.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ContactKeeper.Ineterfaces
{
    public class UserContactRepository : IUserContactRepository
    {
        private readonly DataContext _context;
        private readonly IMemoryCache _cache;

        public UserContactRepository(DataContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }
        public async Task<UserContact> AddUserContact(UserContact userContact)
        {
            string path = "d:/GitHub/Dotnet/APIS/ContactKeeper/util/ddd/dddsBrasileiros.json";
            var jsonString = await File.ReadAllTextAsync(path);
            var dddsData = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, List<string>>>>(jsonString);

            if (dddsData != null && dddsData["dddsPorEstado"].Values.Any(list => list.Contains(userContact.DDD.ToString())))
            {

                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == userContact.UserId);

                if (user == null || user.Id != userContact.UserId)
                {
                    throw new Exception("cant add user contact, user not found");
                }
                _context.UserContacts.Add(userContact);
                await _context.SaveChangesAsync();

                return userContact;


            }
            else
            {
                throw new ArgumentException("DDD inválido.");
            }
        }

        public async Task<IEnumerable<UserContact>> GetUserContact()
        {
            var userContacts = await _context.UserContacts
                .AsNoTracking()
                .ToListAsync();

            return userContacts;
        }

        public async Task<UserContact> DeleteUserContact(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<UserContact> GetUserContactById(int userId)
        {
            var userContact = await _context.UserContacts
                .AsNoTracking()
                .FirstOrDefaultAsync(uc => uc.UserId == userId);
            return userContact;
        }

        public async Task<UserContact> UpdateUserContact(UserContact userContact)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<UserContact>> GetInfoUserByDdd(int ddd)
        {
            const string cacheKey = "UsersByDdd";
            if (!_cache.TryGetValue(cacheKey, out IList<UserContact> userContacts))
            {
                userContacts = await _context.UserContacts
                .AsNoTracking()
                .Where(x => x.DDD == ddd)
                .ToListAsync();

                _cache.Set(cacheKey, userContacts, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
                });
                Console.WriteLine("dados do banco");
            }
            else
            {
                userContacts = _cache.Get(cacheKey) as IList<UserContact>;
                Console.WriteLine("dados do cache");
            }
            return userContacts;
        }
    }
}