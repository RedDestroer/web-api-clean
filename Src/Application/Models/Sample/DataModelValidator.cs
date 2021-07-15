using FluentValidation;

namespace WebApiClean.Application.Models.Sample
{
    public class DataModelValidator : AbstractValidator<DataModel>
    {
        public DataModelValidator()
        {
            RuleFor(o => o.Value).NotEmpty();
        }
    }
}
