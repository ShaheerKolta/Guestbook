using Guestbook.Model;

namespace Guestbook.Interfaces
{
    public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetMessagesAsync();
        Task<Message> GetMessageByIdAsync(int id);
        void CreateMessage(Message message);
        void UpdateMessage(Message message);
        void DeleteMessage(int id);
        Task<IEnumerable<Message>> GetMessagesFromParentByIdAsync(int id);

    }
}
