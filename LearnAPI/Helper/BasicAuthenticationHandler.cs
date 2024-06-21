using LearnAPI.Repos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace LearnAPI.Helper
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly LearndataContext _context;
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, LearndataContext context, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _context = context;
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if(!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("No Header Found");
            }
            var headersValues = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);

            if (headersValues != null)
            {
                var bytes = Convert.FromBase64String(headersValues.Parameter);
                string credentails = Encoding.UTF8.GetString(bytes);
                string[] array = credentails.Split(':');

                string userName = array[0];
                string password = array[1];
                var user = await _context.TblUsers.FirstOrDefaultAsync(x => x.Username == userName && x.Password == password);
                if (user != null)
                {
                    var claim = new[] { new Claim(ClaimTypes.Name, user.Username) };

                    var identity = new ClaimsIdentity(claim, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);
                    return AuthenticateResult.Success(ticket);
                }
                else
                {
                    return AuthenticateResult.Fail("Unauthorized");
                }


            }
            else
            {
                return AuthenticateResult.Fail("Empty Headers");
            }
        }
    }
}
