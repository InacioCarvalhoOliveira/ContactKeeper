using ContactKeeper.Models;

namespace ContactKeeper.Contracts
{
    public interface IUserContactRepository
    {
        Task<IEnumerable<UserContact>> GetUserContact();
        Task<UserContact> GetUserContactById(int id);
        Task<UserContact> AddUserContact(UserContact userContact);
        Task<UserContact> UpdateUserContact(UserContact userContact);
        Task<UserContact> DeleteUserContact(Guid id);
        Task<IList<UserContact>> GetInfoUserByDdd(int ddd);    

    }
}