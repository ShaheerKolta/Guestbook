using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Guestbook.Model
{
    public class Message
    {
        
        [Key]
        public int Message_Id { get; set; }
        [Required]
        public int User_Id { get; set; }
        [ForeignKey("Message_Id")]
        public int? Parent_Id { get; set; }
        [Required]
        public DateTime Creation_Date { get; set; }
        [Required]
        public string Message_Content { get; set; }

        public IEnumerable<Message>? Children { get; set; }

    }
}
