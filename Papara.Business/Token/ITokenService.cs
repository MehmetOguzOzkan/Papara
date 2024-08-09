using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Papara.Data.Entities;

namespace Papara.Business.Token
{
    public interface ITokenService
    {
        Task<string> GenerateToken(User user);
    }
}
