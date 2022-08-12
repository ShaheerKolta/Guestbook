using FluentValidation;
using Guestbook.Model;

namespace Guestbook.Validators
{
    public class MessageValidator : AbstractValidator<Message>
    {
        public MessageValidator()
        {
            RuleFor(message => message.Message_Id).NotNull();
            RuleFor(message => message.User_Id).NotNull();
            RuleFor(message => message.Message_Content).NotNull().MinimumLength(2);
        }
    }
}
