using Guestbook.Interfaces;
using Guestbook.Model;
using Guestbook.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

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
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [Authorize(Policy = "User")]
        public async Task<IActionResult> PostMessage(Message message , [FromHeader] string Authorization)
        {
            //reading of token and making sure message posted by user is same as token holder
            JwtSecurityToken t = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(Authorization.Substring(7));
            var x = t.Claims.ToList();
            if (x[0].Value != message.User_Id.ToString() && x[2].Value != "Admin")
            {
                return Unauthorized(new { message = "User_Id in Message Does not Match User_id in Token" });
            }


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
        [Authorize(Policy = "User")]
        public async Task<IActionResult> EditMessage(Message message , [FromHeader] string Authorization)
        {
            JwtSecurityToken t = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(Authorization.Substring(7));
            var x = t.Claims.ToList();
            if (x[0].Value != message.User_Id.ToString())
            {
                return Unauthorized(new { message = "User_Id in Message Does not Match User_id in Token" });
            }

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
        public async Task<IActionResult> DeleteMessageById(int id, [FromHeader] string Authorization)
        {
            JwtSecurityToken t = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(Authorization.Substring(7));
            var x = t.Claims.ToList();
            

            try
            {
                var message = await _messageRepository.GetMessageByIdAsync(id);
                if (x[0].Value != message.User_Id.ToString() && x[2].Value != "Admin")
                {
                    return Unauthorized(new { message = "User_Id in Message Does not Match User_id in Token" });
                }

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
