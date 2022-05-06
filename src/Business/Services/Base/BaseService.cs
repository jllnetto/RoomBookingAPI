using Business.Interfaces.Notifications;
using Business.Models.Base;
using Business.Notifications;
using FluentValidation;
using FluentValidation.Results;

namespace Business.Services.Base
{
    public class BaseService
    {
        protected readonly INotificator _notificator;

        public BaseService(INotificator notificador)
        {
            _notificator = notificador;
        }

        protected void Notificate(ValidationResult validationResult)
        {
            foreach (var error in validationResult.Errors)
            {
                Notificate(error.ErrorMessage);
            }
        }

        protected void Notificate(string mensagem)
        {
            _notificator.Handle(new Notification(mensagem));
        }

        protected bool ExecuteValidation<TV, TE>(TV validacao, TE entidade) where TV : AbstractValidator<TE> where TE : Entity
        {
            var validator = validacao.Validate(entidade);

            if (validator.IsValid)
            {
                return true;
            }

            Notificate(validator);

            return false;
        }
    }
}
