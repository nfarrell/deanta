using Deanta.Web.Models;
using FluentValidation;

namespace Deanta.Web.Infrastructure.Validators
{
    public partial class ToDoCreateValidator : AbstractValidator<ToDoCreateModel>
    {
        public ToDoCreateValidator()
        {
            RuleFor(x => x.Description).NotNull();
        }
    }
}
