using LearnAPI.Helper;
using LearnAPI.Modal;
using LearnAPI.Repos;
using LearnAPI.Repos.Models;
using LearnAPI.Service;
using Microsoft.EntityFrameworkCore;

namespace LearnAPI.Container
{
    public class UserRoleService : IUserRoleService
    {
        private readonly LearndataContext _learndataContext;

        public UserRoleService(LearndataContext learndataContext)
        {
            _learndataContext = learndataContext;
        }
        public async Task<APIResponse> AssignRolePermission(List<TblRolepermission> _data)
        {
           APIResponse response = new APIResponse();
            int processCount = 0;
            try
            {
                using(var dbtransaction =  await _learndataContext.Database.BeginTransactionAsync())
                {
                    if(_data.Count> 0)
                    {
                        _data.ForEach(item =>
                        {
                            var userData = _learndataContext.TblRolepermissions.FirstOrDefault(item1 => item1.Userrole == item.Userrole &&
                            item1.Menucode == item.Menucode);
                            if(userData != null)
                            {
                                userData.Haveview = item.Haveview;
                                userData.Haveadd = item.Haveadd;
                                userData.Haveedit = item.Haveedit;
                                userData.Havedelete = item.Havedelete;
                                processCount++;

                            }
                            else
                            {
                                _learndataContext.TblRolepermissions.Add(item);
                                processCount++;
                            }

                        });

                        if (_data.Count == processCount)
                        {
                            await _learndataContext.SaveChangesAsync();
                        
                            await dbtransaction.CommitAsync();
                            response.Result = "Pass";
                            response.Message = "Saved successfully.";
                        }
                        else
                        {
                            await dbtransaction.RollbackAsync();
                        }

                    }
                    else
                    {
                        response.Result = "fail";
                        response.Message = "Failed";
                    }
                }
              

            }
            catch (Exception ex)
            {
                response = new APIResponse();
            }
            return response;
        }

        public async Task<List<Appmenu>> GetAllMenubyrole(string userrole)
        {
            List<Appmenu> appmenus = new List<Appmenu>();

            var accessdata = (from menu in _learndataContext.TblRolepermissions.Where(o => o.Userrole == userrole && o.Haveview)
                              join m in _learndataContext.TblMenus on menu.Menucode equals m.Code into _jointable
                              from p in _jointable.DefaultIfEmpty()
                              select new { code = menu.Menucode, name = p.Name }).ToList();
            if (accessdata.Any())
            {
                accessdata.ForEach(item =>
                {
                    appmenus.Add(new Appmenu()
                    {
                        code = item.code,
                        Name = item.name
                    });
                });
            }

            return appmenus;
        }

        public async Task<List<TblMenu>> GetAllMenus()
        {
            return await _learndataContext.TblMenus.ToListAsync();
        }

        public async Task<List<TblRole>> GetAllRoles()
        {
            return await _learndataContext.TblRoles.ToListAsync();
        }
       

        public async Task<Menupermission> GetMenupermissionbyrole(string userrole, string menucode)
        {

            Menupermission menupermission = new Menupermission();
            var _data = await _learndataContext.TblRolepermissions.FirstOrDefaultAsync(o => o.Userrole == userrole && o.Haveview
            && o.Menucode == menucode);
            if (_data != null)
            {
                menupermission.code = _data.Menucode;
                menupermission.Haveview = _data.Haveview;
                menupermission.Haveadd = _data.Haveadd;
                menupermission.Haveedit = _data.Haveedit;
                menupermission.Havedelete = _data.Havedelete;
            }
            return menupermission;
        }
    }
}
