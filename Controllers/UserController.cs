using Guestbook.Interfaces;
using Guestbook.Model;
using Guestbook.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace Guestbook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserRepository _userRepository;
        private readonly IConfiguration _config;
        private UserValidator validator ;
        
        //to be used with sign in
        public record AuthenticationData(string? Email, string? Password);
        public UserController(IUserRepository userRepository , IConfiguration config)
        {
            _userRepository = userRepository;
            validator = new UserValidator();
            _config = config;
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
                var result = validator.Validate(user);
                if (result.IsValid) { 
                    var duplicate = await _userRepository.GetUserByEmailAsync(user.Email);
                    if (duplicate != null)
                        return BadRequest("Email Already Exists");
                    else
                    {
                        //hashing user password to save it in database
                        user.Password = Hashing.Hashing.getHash(user.Password);
                        _userRepository.CreateUser(user);
                        return NoContent();
                    }
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


        //data Recieved must be valid from frontend (All fields must be present except for userId)
        [HttpPut("{email}")]
        public async Task<IActionResult> EditUser(string email ,User user)
        {
            try
            {
                if (email != user.Email)
                    return BadRequest("Emails Doesn't Match");
                //user should be validated here as well but it was not required

                //user password is hashed to be stored in database
                user.Password = Hashing.Hashing.getHash(user.Password);
                _userRepository.UpdateUser(user);
                return NoContent();
            }
            catch
            {
                return BadRequest("Something went Wrong !");
            }
        }



        private string GenerateToken(int userId)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                _config.GetValue<string>("Authentication:SecretKey")));


            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);


            List<Claim> claims = new();
            claims.Add(new(JwtRegisteredClaimNames.Sub, userId.ToString()));



            var token = new JwtSecurityToken(
                config.GetValue<string>("Authentication:Issuer"),
                config.GetValue<string>("Authentication:Audience"),
                claims,
                DateTime.Now,
                DateTime.Now.AddDays(1),
                signingCredentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
