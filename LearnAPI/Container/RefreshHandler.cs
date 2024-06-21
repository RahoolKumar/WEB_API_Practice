using LearnAPI.Repos;
using LearnAPI.Repos.Models;
using LearnAPI.Service;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace LearnAPI.Container
{
    public class RefreshHandler : IRefreshHandler
    {
        private readonly LearndataContext _learndataContext;
        public RefreshHandler(LearndataContext learndataContext)
        {
            _learndataContext = learndataContext;
        }
        public async Task<string> GenerateToken(string userName)
        {
            var randomnumber = new byte[32];
            using(var randomnumbergenerator = RandomNumberGenerator.Create())
            {
                randomnumbergenerator.GetBytes(randomnumber);
                string refreshToken = Convert.ToBase64String(randomnumber);

                var ExistToken = _learndataContext.TblRefreshtokens.FirstOrDefaultAsync(x=>x.Userid == userName).Result;
                if(ExistToken != null)
                {
                    ExistToken.Refreshtoken = refreshToken;
                }
                else
                {
                    await _learndataContext.TblRefreshtokens.AddAsync(new TblRefreshtoken
                    {
                        Userid = userName,
                        Tokenid = new Random().Next().ToString(),
                        Refreshtoken = refreshToken
                    });
                }
                await _learndataContext.SaveChangesAsync();
                return refreshToken;
            }
        }
    }
}
