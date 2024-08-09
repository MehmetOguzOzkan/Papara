using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Session
{
    public static class JwtManager
    {
        public static Session GetSession(HttpContext context)
        {
            Session session = new Session();

            var identity = context.User.Identity as ClaimsIdentity;
            var claims = identity.Claims;

            session.UserName = GetClaim(claims, "UserName");
            session.UserId = GetClaim(claims, "UserId");
            session.Role = GetClaim(claims, "Role");
            session.Email = GetClaim(claims, "Email");

            return session;
        }

        private static string GetClaim(IEnumerable<Claim> claims, string name)
        {
            var claim = claims.FirstOrDefault(x => x.Type == name);
            return (claim != null) ? claim.Value : string.Empty;
        }
    }
}
