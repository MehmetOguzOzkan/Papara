using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Papara.Business.Notification
{
    public interface INotificationService
    {
        public void SendEmail(string subject, string email, string content);
    }
}
