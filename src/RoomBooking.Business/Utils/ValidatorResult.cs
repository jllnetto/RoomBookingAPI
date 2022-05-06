using RoomBooking.Business.Interfaces.Notifications;
using RoomBooking.Business.Notifications;

namespace RoomBooking.Business.Utils
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
