using LearnAPI.Modal;
using LearnAPI.Repos;
using LearnAPI.Repos.Models;
using LearnAPI.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearnAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
       
        public UserController(IUserService userService)
        {
            _userService = userService;
         
        }

        [HttpPost("userregistration")]
        public async Task<IActionResult> UserRegistration(UserRegister userRegister)
        {
            var data = await _userService.UserRegisteration(userRegister);
            return Ok(data);
        }
        [HttpPost("confirmregistration")]
        public async Task<IActionResult> confirmregistration(Confirmpassword _data)
        {
            var data = await _userService.ConfirmRegister(_data);
            return Ok(data);
        }
        [HttpPost("resetpassword")]
        public async Task<IActionResult> resetpassword(Resetpassword _data)
        {
            var data = await _userService.ResetPassword(_data.username, _data.oldpassword, _data.newpassword);
            return Ok(data);
        }
        [HttpGet("forgetpassword")]
        public async Task<IActionResult> forgetpassword(string username)
        {
            var data = await _userService.ForgetPassword(username);
            return Ok(data);
        }

        [HttpPost("updatepassword")]
        public async Task<IActionResult> updatepassword(Updatepassword _data)
        {
            var data = await _userService.UpdatePassword(_data.username, _data.password, _data.otptext);
            return Ok(data);
        }
        [HttpPost("updatestatus")]
        public async Task<IActionResult> updatestatus(Updatestatus _data)
        {
            var data = await _userService.UpdateStatus(_data.username, _data.status);
            return Ok(data);
        }

        [HttpPost("updaterole")]
        public async Task<IActionResult> updaterole(UpdateRole _data)
        {
            var data = await _userService.UpdateRole(_data.username, _data.role);
            return Ok(data);
        }



    }
}
