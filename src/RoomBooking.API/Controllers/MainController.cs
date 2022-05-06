using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RoomBooking.Business.Interfaces.Notifications;
using RoomBooking.Business.Notifications;

namespace RoomBooking.API.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        private readonly INotificator _notificator;

        protected MainController(INotificator notificator)
        {
            _notificator = notificator;
        }

        protected bool ValidOperation()
        {
            return !_notificator.HaveNotifications();
        }

        protected ActionResult CustomResponse(object result = null)
        {
            if (ValidOperation())
            {
                return Ok(new
                {
                    success = true,
                    data = result
                });
            }

            return BadRequest(new
            {
                success = false,
                errors = _notificator.GetNotifications().Select(n => n.Message)
            });
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid) InvalidModelNotificationError(modelState);
            return CustomResponse();
        }

        protected void InvalidModelNotificationError(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(e => e.Errors);
            foreach (var erro in erros)
            {
                var errorMsg = erro.Exception is null ? erro.ErrorMessage : erro.Exception.Message;
                NotificateError(errorMsg);
            }
        }

        protected void NotificateError(string mensagem)
        {
            _notificator.Handle(new Notification(mensagem));
        }
    }
}
