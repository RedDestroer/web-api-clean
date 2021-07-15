using FluentValidation;

namespace WebApiClean.Application.Samples.Queries.Sample
{
    public class SampleQueryValidator : AbstractValidator<SampleQuery>
    {
        public SampleQueryValidator()
        {
            RuleFor(o => o).NotNull();
            RuleFor(o => o.Echo).NotNull();
        }
    }
}
