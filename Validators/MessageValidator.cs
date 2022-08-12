using FluentValidation;
using Guestbook.Model;

namespace Guestbook.Validators
{
    public class MessageValidator : AbstractValidator<Message>
    {
        public MessageValidator()
        {
            RuleFor(message => message.Message_Id).NotEmpty().NotNull().NotEqual(0);
            RuleFor(message => message.User_Id).NotNull().NotEqual(0);
            RuleFor(message => message.Message_Content).NotNull().MinimumLength(2);
        }

        //for overloading purpose only I have added an int
        public MessageValidator(int operation)
        {
            RuleFor(message => message.User_Id).NotNull().NotEqual(0);
            RuleFor(message => message.Message_Content).NotNull().MinimumLength(2);
        }
    }
}
