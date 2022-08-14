using Guestbook.Model;

namespace Guestbook.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetUsersAsync();
        void CreateUser(User user);

        //state is used to flag wether password is changed in this edit or not , states are provided in implemtntation
        void UpdateUser(User user , int state);
        void DeleteUserAsync(int id);
    }
}
