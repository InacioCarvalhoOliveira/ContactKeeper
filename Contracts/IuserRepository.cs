using ContactKeeper.Models;

namespace ContactKeeper.Contracts
{
    public interface IuserRepository
    {
        Task<User> GetUserById(int id);      
        Task<IEnumerable<User>> GetUsers();
        Task GetUserByPhoneNumber(PhoneNumber phoneNumber);
        Task<User> AddUser(User user);
        Task<User> UpdateUser(User user);
        Task<User> DeleteUser(User user);
    }
}