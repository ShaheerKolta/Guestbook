using Guestbook.Interfaces;
using Guestbook.Model;
using Guestbook.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
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
        public record AuthenticationData(string Email, string Password);
        public UserController(IUserRepository userRepository , IConfiguration config)
        {
            _userRepository = userRepository;
            validator = new UserValidator();
            _config = config;
        }


        //Function to get All Users to be used by admin
        [HttpGet]
        [Authorize(Policy = "Admin")]
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
        [Authorize(Policy = "User")]
        public async Task<ActionResult<User>> GetUserByEmail(string email , [FromHeader] string Authorization)
        {
            JwtSecurityToken t = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(Authorization.Substring(7));
            var x = t.Claims.ToList();

            if (x[1].Value != email && x[2].Value != "Admin")
            {
                return Unauthorized(new { message = "Emails Does not Match" });
            }
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
        [Authorize(Policy = "User")]
        public async Task<IActionResult> DeleteUserByEmail(string email , [FromHeader] string Authorization)
        {
            JwtSecurityToken t = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(Authorization.Substring(7));
            var x = t.Claims.ToList();

            if (x[1].Value != email && x[2].Value != "Admin")
            {
                return Unauthorized(new { message = "Emails Does not Match" });
            }
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
        [AllowAnonymous]
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
        [Authorize(Policy ="User")]
        public async Task<IActionResult> EditUser(string email ,User user , [FromHeader] string Authorization)
        {
            JwtSecurityToken t = (JwtSecurityToken)new JwtSecurityTokenHandler().ReadToken(Authorization.Substring(7));
            var x = t.Claims.ToList();

            if (x[1].Value != email && x[2].Value != "Admin")
            {
                return Unauthorized(new {message = "Emails Does not Match"});
            }
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

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<string>> Authenticate([FromBody] AuthenticationData data)
        {
            try
            {
                User user = await _userRepository.GetUserByEmailAsync(data.Email);
                if (user == null)
                    return BadRequest(new { message = "User Does not Exist" });
                var hashedPassword = Hashing.Hashing.getHash(data.Password);
                if(!user.Password.Equals(hashedPassword))
                    return BadRequest(new {message ="Wrong Password"});
                String role = "";
                if (user.User_Id == 1)
                    role = "Admin";
                else role = "User";
                var token = GenerateToken(user , role);
                return Ok(new { token = token, userId = user.User_Id});
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }


        private string GenerateToken(User user , string role)
        {
            var secretKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                _config.GetValue<string>("Authentication:SecretKey")));


            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);


            List<Claim> claims = new();
            claims.Add(new(JwtRegisteredClaimNames.Sub, user.User_Id.ToString()));
            claims.Add(new(JwtRegisteredClaimNames.UniqueName, user.Email.ToString()));
            claims.Add(new("Role", role));



            var token = new JwtSecurityToken(
                _config.GetValue<string>("Authentication:Issuer"),
                _config.GetValue<string>("Authentication:Audience"),
                claims,
                DateTime.Now,
                DateTime.Now.AddDays(1),
                signingCredentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
