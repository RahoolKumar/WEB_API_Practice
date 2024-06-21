using AutoMapper.Internal;
using LearnAPI.Helper;
using LearnAPI.Modal;
using LearnAPI.Repos;
using LearnAPI.Repos.Models;
using LearnAPI.Service;
using Microsoft.EntityFrameworkCore;

namespace LearnAPI.Container
{
    public class UserService : IUserService
    {
        private readonly LearndataContext _learndataContext;
        private readonly IEmailService _emailService;
        public UserService(LearndataContext learndataContext , IEmailService emailService)
        {
            _learndataContext = learndataContext;
            _emailService = emailService;

        }
        public async Task<APIResponse> ConfirmRegister(Confirmpassword _data)
        {
            APIResponse response = new APIResponse();
            bool otpresponse = await ValidateOTP(_data.username, _data.otptext);
            if (!otpresponse)
            {
                response.Result = "Fail";
                response.Message = "Invalid OTP or Expired";
            }
            else
            {
                var tempData = await  _learndataContext.TblTempusers.FirstOrDefaultAsync(item => item.Id == _data.userid);
                var _user = new TblUser()
                {
                    Username = _data.username,
                    Name = tempData.Name,
                    Password = tempData.Password,
                    Email   = tempData.Email,
                    Phone = tempData.Phone,
                    Failattempt = 0,
                    Isactive = true,
                    Islocked = false,
                    Role = "user"

                };
                await _learndataContext.TblUsers.AddAsync(_user);
                await _learndataContext.SaveChangesAsync();
                await UpdatePWDManager(_data.username, tempData.Password);

                response.Result = "pass";
                response.Message = "Registered Successfully";
            
            
            }
            return response;
        
        }
        private async Task<bool> Validatepwdhistory(string Username, string password)
        {
            bool response = false;
            var _pwd = await _learndataContext.TblPwdMangers.Where(item => item.Username == Username).
                OrderByDescending(p => p.ModifyDate).Take(3).ToListAsync();
            if (_pwd.Count > 0)
            {
                var validate = _pwd.Where(o => o.Password == password);
                if (validate.Any())
                {
                    response = true;
                }
            }

            return response;

        }
        public async Task<APIResponse> ResetPassword(string username, string oldpassword, string newpassword)
        {
            APIResponse response = new APIResponse();
            var _user = await _learndataContext.TblUsers.FirstOrDefaultAsync(item => item.Username == username &&
            item.Password == oldpassword && item.Isactive == true);
            if (_user != null)
            {
                var _pwdhistory = await Validatepwdhistory(username, newpassword);
                if (_pwdhistory)
                {
                    response.Result = "fail";
                    response.Message = "Don't use the same password that used in last 3 transaction";
                }
                else
                {
                    _user.Password = newpassword;
                    await _learndataContext.SaveChangesAsync();
                    await UpdatePWDManager(username, newpassword);
                    response.Result = "pass";
                    response.Message = "Password changed.";
                }
            }
            else
            {
                response.Result = "fail";
                response.Message = "Failed to validate old password.";
            }
            return response;
        }
        public async Task<APIResponse> UserRegisteration(UserRegister userRegister)
         {
            APIResponse response = new APIResponse();
            int userId = 0;
            bool isValid = true;
            try
            {
                // duplicate user

                var _user = await _learndataContext.TblUsers.Where(item => item.Username == userRegister.UserName).ToListAsync();
                if(_user.Count >0)
                {
                    isValid = false;
                    response.Result = "Fail";
                    response.Message = "Dupicate Username";
                }

                var _useremail = await _learndataContext.TblUsers.Where(item => item.Email == userRegister.Email).ToListAsync();
                if (_useremail.Count > 0)
                {
                    isValid = false;
                    response.Result = "Fail";
                    response.Message = "Dupicate Email";
                }


                if (userRegister!=null && isValid)
                {
                    var tempUser = new TblTempuser()
                    {
                        Code = userRegister.UserName, Password = userRegister.Password,
                        Name = userRegister.Name,
                        Email = userRegister.Email,
                        Phone = userRegister.Phone

                    };


                 await _learndataContext.TblTempusers.AddAsync(tempUser);
                 await _learndataContext.SaveChangesAsync();
                
                    userId = tempUser.Id;

                    string OTPText = GenerateRandom();
                    await UpdateOtp(userRegister.UserName, OTPText, "register");
                    await SendOtpMail(userRegister.Email,OTPText,userRegister.Name);
                    response.Result = "Pass";
                    response.Message = userId.ToString();
                 

                }
            }
            catch (Exception ex)
            {
                response.Result = "Fail";
                response = new APIResponse();
            }
            return response;
           

        }

        
        private async Task<bool> ValidateOTP(string username,string OTPText)
        {
            bool response = false;
            var _data = await _learndataContext.TblOtpManagers.FirstOrDefaultAsync(x=>x.Username== username &&
            x.Otptext==OTPText && x.Expiration>DateTime.Now);
            if( _data != null )
            {
                response = true;
            }

            return response;

        }

        private async Task UpdateOtp(string username, string otptext, string otptype)
        {
            var otp = new TblOtpManager()
            {
                Username = username,
                Otptext = otptext,
                Expiration = DateTime.Now.AddMinutes(30),
                Createddate = DateTime.Now,
                Otptype = otptype

            };

            await _learndataContext.TblOtpManagers.AddAsync(otp);
            await _learndataContext.SaveChangesAsync();
        }


        private async Task UpdatePWDManager(string username, string password)
        {
            var _Password = new TblPwdManger()
            {
                Username = username,
                Password = password,
                ModifyDate = DateTime.Now,
            };

            await _learndataContext.TblPwdMangers.AddAsync(_Password);
            await _learndataContext.SaveChangesAsync();
        }

        private string GenerateRandom()
        {
            Random random = new Random();
            string randomNo = random.Next(0,1000000).ToString("D6");
            return randomNo;
        }


        private async Task SendOtpMail(string useremail, string OtpText, string Name)
        {
            var mailrequest = new Mailrequest();
            mailrequest.Email = useremail;
            mailrequest.Subject = "Thanks for registering : OTP";
            mailrequest.Emailbody = GenerateEmailBody(Name, OtpText);
            await this._emailService.SendEmail(mailrequest);

        }
        private string GenerateEmailBody(string name, string otptext)
        {
            string emailbody = "<div style='width:100%;background-color:grey'>";
            emailbody += "<h1>Hi " + name + ", Thanks for registering</h1>";
            emailbody += "<h2>Please enter OTP text and complete the registeration</h2>";
            emailbody += "<h2>OTP Text is :" + otptext + "</h2>";
            emailbody += "</div>";

            return emailbody;
        }

        public async Task<APIResponse> ForgetPassword(string username)
        {
            APIResponse response = new APIResponse();
            var _user = await _learndataContext.TblUsers.FirstOrDefaultAsync(item => item.Username == username && item.Isactive == true);
            if (_user != null)
            {
                string otptext = GenerateRandom();
                await UpdateOtp(username, otptext, "forgetpassword");
                await SendOtpMail(_user.Email, otptext, _user.Name);
                response.Result = "pass";
                response.Message = "OTP sent";

            }
            else
            {
                response.Result = "fail";
                response.Message = "Invalid User";
            }
            return response;
        }

        public async Task<APIResponse> UpdatePassword(string username, string Password, string Otptext)
        {
            APIResponse response = new APIResponse();

            bool otpvalidation = await ValidateOTP(username, Otptext);
            if (otpvalidation)
            {
                bool pwdhistory = await Validatepwdhistory(username, Password);
                if (pwdhistory)
                {
                    response.Result = "fail";
                    response.Message = "Don't use the same password that used in last 3 transaction";
                }
                else
                {
                    var _user = await _learndataContext.TblUsers.FirstOrDefaultAsync(item => item.Username == username && item.Isactive == true);
                    if (_user != null)
                    {
                        _user.Password = Password;
                        await _learndataContext.SaveChangesAsync();
                        await UpdatePWDManager(username, Password);
                        response.Result = "pass";
                        response.Message = "Password changed";
                    }
                }
            }
            else
            {
                response.Result = "fail";
                response.Message = "Invalid OTP";
            }
            return response;
        }

        public async Task<APIResponse> UpdateStatus(string username, bool userstatus)
        {
            APIResponse response = new APIResponse();
            var _user = await _learndataContext.TblUsers.FirstOrDefaultAsync(item => item.Username == username);
            if (_user != null)
            {
                _user.Isactive = userstatus;
                await _learndataContext.SaveChangesAsync();
                response.Result = "pass";
                response.Message = "User Status changed";
            }
            else
            {
                response.Result = "fail";
                response.Message = "Invalid User";
            }
            return response;
        }
        public async Task<APIResponse> UpdateRole(string username, string userrole)
        {
            APIResponse response = new APIResponse();
            var _user = await _learndataContext.TblUsers.FirstOrDefaultAsync(item => item.Username == username && item.Isactive == true);
            if (_user != null)
            {
                _user.Role = userrole;
                await _learndataContext.SaveChangesAsync();
                response.Result = "pass";
                response.Message = "User Role changed";
            }
            else
            {
                response.Result = "fail";
                response.Message = "Invalid User";
            }
            return response;
        }
    }
}
