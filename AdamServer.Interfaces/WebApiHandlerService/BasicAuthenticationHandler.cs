using AdamServer.Interfaces.WebApiHandlerService.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Linq;

namespace AdamServer.Interfaces.WebApiHandlerService
{
    [Obsolete]
    public class BasicAuthenticationHandler :  AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IAuthenticationChecker mAuthenticationChecker;
        public BasicAuthenticationHandler(IAuthenticationChecker authenticationChecker, IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock) 
        {
            mAuthenticationChecker =  authenticationChecker;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string username;

            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);

                if (authHeader.Parameter == null)
                {
                    throw new ArgumentException("Request.Headers[\"Authorization\"] parse error");
                }

                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader.Parameter)).Split(':');

                if (credentials == null)
                {
                    throw new ArgumentException("Credentials is null");
                }

                username = credentials.First();
                var password = credentials.Last();

                if (!mAuthenticationChecker.ValidateCredentials(username, password))
                    throw new ArgumentException("Invalid credentials");
            }
            catch (Exception ex)
            {
                Log.Error($"Authentication failed: {ex.Message}");
                return Task.FromResult(AuthenticateResult.Fail($"Authentication failed: {ex.Message}"));
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
