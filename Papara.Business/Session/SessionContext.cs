using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Session
{
    internal class SessionContext : ISessionContext
    {
        public HttpContext HttpContext { get; set; }
        public Session Session { get; set; }
    }
}
