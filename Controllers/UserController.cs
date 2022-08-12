using Guestbook.Interfaces;
using Guestbook.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Guestbook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        //Function to get All Users to be used by admin
      [HttpGet]
      public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                var users = await _userRepository.GetUsersAsync();
                if (users != null)
                    return Ok(users);
                else return BadRequest(new { Message = "Users Table is Empty !!" });
            }
            catch
            {
                return Problem("Something went Wrong During Excution");
            }
           
        }


        //Function to get user by Emial to be used to display profile
        [HttpGet("{email}")]
        public async Task<ActionResult<User>> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(email);
                if (user != null)
                    return user;
                else return BadRequest(new { Message = "User Not Found !!" });
            }
            catch
            {
                return Problem("Something went Wrong During Excution");
            }
        }

        //to be used by client to delete it's Account
        [HttpDelete("{email}")]
        public async Task<IActionResult> DeleteUserByEmail(string email)
        {
            try
            {
                var user = await _userRepository.GetUserByEmailAsync(email);
                if (user != null)
                {
                    _userRepository.DeleteUserAsync(user.User_Id);
                    return Ok();
                }
                else
                    return BadRequest("User Not Found !");
            }
            catch
            {
                return BadRequest("Something Went Wrong !");
            }
        }

        //used to create an account
        [HttpPost]
        public async Task<IActionResult> PostUser(User user)
        {
            try
            {
                var duplicate = await _userRepository.GetUserByEmailAsync(user.Email);
                if (duplicate != null)
                    return BadRequest("Email Already Exists");
                else
                {
                    _userRepository.CreateUser(user);
                    return NoContent();
                }
            }
            catch
            {
                return BadRequest("Something went Wrong !");
            }
        }


        //data Recieved must be valid from frontend (All fields must be present except for userId)
        [HttpPut("{email}")]
        public async Task<IActionResult> EditUser(string email ,User user)
        {
            try
            {
                if (email != user.Email)
                    return BadRequest("Emails Doesn't Match");
                _userRepository.UpdateUser(user);
                return NoContent();
            }
            catch
            {
                return BadRequest("Something went Wrong !");
            }
        }
    }
}
