using FluentValidation;

namespace WebApiClean.Application.Models.Sample
{
    public class GetAllSampleRequestValidator : AbstractValidator<GetAllSampleRequest>
    {
        public GetAllSampleRequestValidator(IValidator<DataModel> dataModelValidator)
        {
            RuleFor(o => o.Data).NotNull();
            RuleFor(o => o.Data).SetValidator(dataModelValidator);
        }
    }
}
