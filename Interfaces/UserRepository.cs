using ContactKeeper.Contracts;
using ContactKeeper.Models;

namespace ContactKeeper.Interfaces
{
    public class UserRepository : IuserRepository
    {
        public Task<User> AddUser(User user)
        {
            throw new NotImplementedException();
        }

        public Task<User> DeleteUser(User user)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserById(int id)
        {
            throw new NotImplementedException();
        }

        public Task GetUserByPhoneNumber(PhoneNumber phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetUsers()
        {
            throw new NotImplementedException();
        }

        public Task<User> UpdateUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}