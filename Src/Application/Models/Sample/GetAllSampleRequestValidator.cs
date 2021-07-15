using FluentValidation;

namespace WebApiClean.Application.Models.Sample
{
    public class GetAllSampleRequestValidator : AbstractValidator<GetAllSampleRequest>
    {
        public GetAllSampleRequestValidator()
        {
            RuleFor(o => o.Data).NotNull();
            RuleFor(o => o.Data).InjectValidator();
        }
    }
}
