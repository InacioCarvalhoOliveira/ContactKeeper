using ContactKeeper.Models;

namespace ContactKeeper.Contracts
{
    public interface IuserRepository
    {
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUserById(int id);      
        Task<IList<User>> GetInfoUserByDdd(int ddd);
        Task<User> AddUser(User user);
        Task<User> UpdateUser(User user);
        Task<User> DeleteUser(int id);
    }
}