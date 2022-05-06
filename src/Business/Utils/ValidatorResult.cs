using Business.Interfaces.Notifications;
using Business.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Utils
{
    public class ValidatorResult
    {
        private INotificator _notificator;
        public bool IsValid { get; set; }
        public List<string> Messages { get; }
        public ValidatorResult(INotificator notificator)
        {
            _notificator = notificator;
            Messages = new List<string>();
        }

        public void AddMessage(string message)
        {
            _notificator.Handle(new Notification(message));
            Messages.Add(message);
        }
        
    }
}
