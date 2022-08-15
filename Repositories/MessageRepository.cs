using Dapper;
using FluentValidation;
using Guestbook.Dapper;
using Guestbook.Interfaces;
using Guestbook.Model;

namespace Guestbook.Repositories
{
    public class MessageRepository : AbstractValidator<Message>, IMessageRepository
    {
        private readonly DapperContext _context;
        public MessageRepository(DapperContext context)
        {
            _context = context;

        }
        

        public async Task<IEnumerable<Message>> GetMessagesAsync()
        {
            var query = "SELECT * FROM Message WHERE Parent_Id is NULL order by Creation_Date";
            //var childrenQuery = "SELECT * FROM Message WHERE Parent_Id=@Message_Id order by Creation_Date";
            using (var connection = _context.CreateConnection())
            {
                var messages = await connection.QueryAsync<Message>(query);

                //This Approach has major impact on response but can be used if performance is optimised
               /* foreach (var message in messages)
                {
                    message.Children = await connection.QueryAsync<Message>(childrenQuery, new {message.Message_Id});
                }*/
                return messages;
            }
        }

        public async Task<Message> GetMessageByIdAsync(int id)
        {
            var query = "SELECT * FROM Message WHERE Message_Id=@Id";
            using (var connection = _context.CreateConnection())
            {
                var message = await connection.QueryFirstOrDefaultAsync<Message>(query, new {id});
                return message;
            }
        }

        /*Replies are fetched in Ascending order of creation
         * ASSUMPTION : the Frontend would show all the main messages only , if a user wishes to see the replies he shall click on the message to retrive them
         */
        public async Task<IEnumerable<Message>> GetMessagesFromParentByIdAsync(int id)
        {
            var query = "SELECT * FROM Message WHERE Parent_Id=@Id order by Creation_Date";
            using (var connection = _context.CreateConnection())
            {
                var messages = await connection.QueryAsync<Message>(query, new {id});
                return messages;
            }
        }



        public void CreateMessage(Message message)
        {
            var query = "INSERT INTO Message(User_Id,Parent_Id,Creation_Date,Message_Content) Values(@User_Id,@Parent_Id,@Creation_Date,@Message_Content)";
            using (var connection = _context.CreateConnection())
            {
                message.Creation_Date = DateTime.Now;
                connection.Query<Message>(query, message);
            }
        }

        public void DeleteMessage(int id)
        {
            //delete message and all it's replies
            var query = "Delete FROM Message WHERE Message_Id=@Id or Parent_Id=@id";
            using (var connection = _context.CreateConnection())
            {
                var message = connection.Execute(query, new { id });
            }
        }

        public void UpdateMessage(Message message)
        {
            var query = "UPDATE Message SET Message_Content=@Message_Content WHERE Message_Id=@Message_Id";

            using (var connection = _context.CreateConnection())
            {
                connection.Query(query, message);
            }
        }
    }
}
