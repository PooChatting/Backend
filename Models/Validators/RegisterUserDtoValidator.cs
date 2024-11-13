using FluentValidation;
using Poochatting.DbContext;
using Poochatting.Exceptions;

namespace Poochatting.Models.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator(MessageDbContext dbcontext) 
        { 
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password).MinimumLength(8);

            RuleFor(x => x.ConfirmPassword).Equal(e => e.Password);

            RuleFor(x => x.Email).Custom((value, context) =>
            {
                var emailInUse = dbcontext.Users.Any(u => u.Email == value);
                if (emailInUse)
                {
                    throw new BadRequestException("Email already taken");
                }
            });
        }
    }
}
