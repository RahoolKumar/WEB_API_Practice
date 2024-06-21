using LearnAPI.Container;
using LearnAPI.Modal;
using LearnAPI.Repos.Models;
using LearnAPI.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearnAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly IUserRoleService _userRoleService;
        public UserRoleController(IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }
        [HttpPost("assignrolepermission")]
        public async Task<IActionResult> assignrolepermission(List<TblRolepermission> rolepermissions)
        {
            var data = await _userRoleService.AssignRolePermission(rolepermissions);
             return Ok(data);
        }
        [HttpGet("GetAllRoles")]
        public async Task<IActionResult> GetAllRoles()
        {
            var data = await this._userRoleService.GetAllRoles();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet("GetAllMenus")]
        public async Task<IActionResult> GetAllMenus()
        {
            var data = await this._userRoleService.GetAllMenus();
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
        [HttpGet("GetAllMenusbyrole")]
        public async Task<IActionResult> GetAllMenusbyrole(string userrole)
        {
            var data = await this._userRoleService.GetAllMenubyrole(userrole);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [HttpGet("GetMenupermissionbyrole")]
        public async Task<IActionResult> GetMenupermissionbyrole(string userrole, string menucode)
        {
            var data = await this._userRoleService.GetMenupermissionbyrole(userrole, menucode);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
    }

}
