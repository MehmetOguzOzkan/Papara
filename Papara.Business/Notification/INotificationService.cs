using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Notification
{
    public interface INotificationService
    {
        Task SendEmail(string to, string subject, string body);
    }
}
