using RoomBooking.Business.Notifications;

namespace RoomBooking.Business.Interfaces.Notifications
{
    public interface INotificator
    {
        public bool HaveNotifications();

        List<Notification> GetNotifications();

        public void Handle(Notification notification);
    }
}
