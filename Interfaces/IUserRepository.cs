using Guestbook.Model;

namespace Guestbook.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetUsersAsync();
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUserAsync(int id);
    }
}
