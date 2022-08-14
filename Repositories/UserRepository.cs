using Dapper;
using Guestbook.Dapper;
using Guestbook.Interfaces;
using Guestbook.Model;

namespace Guestbook.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;
        public UserRepository(DapperContext context)
        {
            _context = context;
        }
        

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var query = "SELECT * FROM Users WHERE Email=@Email";
            using (var connection = _context.CreateConnection())
            {
                var user = await connection.QuerySingleOrDefaultAsync<User>(query, new { email });
                return user;
            }

        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var query = "SELECT User_Id,Name,Date_of_Birth,Email FROM Users";
            using (var connection = _context.CreateConnection())
            {
                var user = await connection.QueryAsync<User>(query);
                if (user == null)
                    return null;
                return user.ToList();
            }

        }

        /*Crucial Data are the ones to be used in insertion only 
         * 
         */

        public void CreateUser(User user)
        {
            var query = "INSERT INTO Users(Name,Date_of_Birth,Email,Password) Values(@Name,@Date_of_Birth,@Email,@Password)";
            using (var connection = _context.CreateConnection())
            {
                connection.Query<User>(query, user);
            }
        }



        public void DeleteUserAsync(int id)
        {
            var query = "Delete FROM Users WHERE User_Id=@Id";
            using (var connection = _context.CreateConnection())
            {
                var user = connection.Execute(query, new { id });
            }
        }


        /*ASSUMPTIONS : User Cannot Change Email
         * User Id must be sent 
         * state : 0 password is changed , else : password is not changed
         */
        public void UpdateUser(User user , int state)
        {
            if (state == 0)
            {
                var query = "UPDATE Users SET Name=@Name,Date_of_Birth =@Date_of_Birth,Password=@Password WHERE Email=@Email";
                using (var connection = _context.CreateConnection())
                {
                    connection.Query(query, user);
                }
            }
            else
            {
                var query = "UPDATE Users SET Name=@Name,Date_of_Birth =@Date_of_Birth WHERE Email=@Email";
                using (var connection = _context.CreateConnection())
                {
                    connection.Query(query, user);
                }
            }
        }
    }
}
