using Guestbook.Interfaces;
using Guestbook.Model;
using Guestbook.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guestbook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private IMessageRepository _messageRepository;
        public MessageController(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
        {
            try
            {
                var messages = await _messageRepository.GetMessagesAsync();
                if (messages == null)
                    return NotFound("Messages are empty");
                return Ok(messages);
            }
            catch
            {
                return BadRequest("No Messages Found");
            }
        }

        //id is the Message_Id
        [HttpGet("replies/{id}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessagesReplies(int id)
        {
            try
            {
                var messages = await _messageRepository.GetMessagesFromParentByIdAsync(id);
                if (messages == null)
                    return NotFound("Message has no replies");
                return Ok(messages);
            }
            catch
            {
                return BadRequest("No Messages Found");
            }
        }



        /*In case of reply the Parent_Id attribute of the message shall be set with Message_Id of the message that the reply is made to
         */
        [HttpPost]
        public async Task<IActionResult> PostMessage(Message message)
        {
            try
            {
                MessageValidator validator = new MessageValidator(1);
                var result =validator.Validate(message);
                if (result.IsValid)
                {
                    _messageRepository.CreateMessage(message);
                    return NoContent();
                }
                else
                {
                    return BadRequest(result.ToString(" - "));
                }

                
                
            }
            catch
            {
                return BadRequest("Something went Wrong !");
            }
        }




        //Essential Data : Message_Id and Message_Content
        //To do : check Task
        [HttpPut]
        public async Task<IActionResult> EditMessage(Message message)
        {
            try
            {
                MessageValidator validator = new MessageValidator();
                var result = validator.Validate(message);
                if (result.IsValid)
                {
                    _messageRepository.UpdateMessage(message);
                    return NoContent();
                }
                else { return BadRequest(result.ToString(" - ")); }
            }
            catch
            {
                return BadRequest("Something went Wrong !");
            }
        }





        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessageById(int id)
        {
            try
            {
                var message = await _messageRepository.GetMessageByIdAsync(id);
                if (message != null)
                {
                    _messageRepository.DeleteMessage(id);
                    return Ok();
                }
                else
                    return BadRequest("Message Not Found !");
            }
            catch
            {
                return BadRequest("Something Went Wrong !");
            }
        }

    }
}
