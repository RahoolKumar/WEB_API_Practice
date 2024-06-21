using LearnAPI.Modal;
using LearnAPI.Repos;
using LearnAPI.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;

namespace LearnAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizeController : ControllerBase
    {
        private readonly LearndataContext _context;
        private readonly JwtSettings _jwtSettings;
        private readonly IRefreshHandler _refresh;
        public AuthorizeController(LearndataContext context,IOptions<JwtSettings> options,IRefreshHandler refreshHandler)
        {
            
            _context = context;
            _jwtSettings = options.Value;
            _refresh = refreshHandler;
        
        }

       
        [HttpPost("GenerateToken")]
        public async Task<IActionResult> GenerateToken([FromBody] UserCred userCred)
        {
            var user = await _context.TblUsers.FirstOrDefaultAsync(x => x.Username == userCred.username && x.Password == userCred.password);
            if (user != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(_jwtSettings.securitykey);
                var tokendesc = new SecurityTokenDescriptor
                {
                    Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Role, user.Role)
                    }),
                    Expires = DateTime.UtcNow.AddSeconds(30),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
                };

                var token =  tokenHandler.CreateToken(tokendesc);
                var finalToken = tokenHandler.WriteToken(token);
                return Ok(new TokenResponse() { Token=finalToken,RefreshToken=await _refresh.GenerateToken(userCred.username)});

            }
            else
            {
                return Unauthorized();
            }

                
        }
        [HttpPost("GenerateRefreshToken")]
        public async Task<IActionResult> GenerateToken([FromBody] TokenResponse token)
        {
            var _refreshToken = await _context.TblRefreshtokens.FirstOrDefaultAsync(x => x.Refreshtoken == token.RefreshToken);
            if (_refreshToken != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(_jwtSettings.securitykey);

                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(token.Token, new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                }, out securityToken);

                var _token = securityToken as JwtSecurityToken;
                if (_token != null  && _token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))  
                {
                    string username = principal.Identity?.Name;
                    var _existdata = await _context.TblRefreshtokens.FirstOrDefaultAsync(item=>item.Userid == username 
                    && item.Refreshtoken==token.RefreshToken);
                    if(_existdata != null )
                    {
                        var _newToken = new JwtSecurityToken(
                            claims: principal.Claims.ToArray(),
                            expires: DateTime.Now.AddSeconds(30),
                            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.securitykey)),
                            SecurityAlgorithms.HmacSha256)
                            );

                        var _finalToken = tokenHandler.WriteToken(_newToken);
                        return Ok(new TokenResponse() { Token = _finalToken, RefreshToken = await _refresh.GenerateToken(username) });

                    }
                    else
                    {
                        return Unauthorized();
                    }
                }
                else
                {
                    return Unauthorized();
                }
                //var tokendesc = new SecurityTokenDescriptor
                //{
                //    Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
                //    {
                //        new Claim(ClaimTypes.Name, user.Username),
                //        new Claim(ClaimTypes.Role, user.Role)
                //    }),
                //    Expires = DateTime.UtcNow.AddSeconds(30),
                //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
                //};

                //var token = tokenHandler.CreateToken(tokendesc);
                //var finalToken = tokenHandler.WriteToken(token);
                //return Ok(new TokenResponse() { Token = finalToken, RefreshToken = await _refresh.GenerateToken(userCred.username) });

            }
            else
            {
                return Unauthorized();
            }


        }
    }
}
