using ContactKeeper.Models;

namespace ContactKeeper.Services.Repositories
{
    public interface IuserRepository
    {
        Task<List<User>> GetUsers();
        Task<User> GetUserById(int id);      
        Task<User> AddUser(User user);
        Task<UserDto> Authenticate(string username, string role, string password);
        Task<User> UpdateUser(User user);
        Task<User> DeleteUser(int id);
    }
}