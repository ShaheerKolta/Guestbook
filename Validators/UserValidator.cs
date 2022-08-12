using FluentValidation;
using Guestbook.Model;

namespace Guestbook.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(user => user.Name).NotNull();
            RuleFor(user => user.Date_of_Birth).NotNull();
            RuleFor(user => user.Email).NotNull().EmailAddress().Matches(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            RuleFor(user => user.Password).NotNull().MinimumLength(4).MaximumLength(10).Matches(@"[0-9]+").Matches(@"[A-Z]+");
        }
    }
}
