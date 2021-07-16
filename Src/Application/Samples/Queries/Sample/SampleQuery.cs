using AutoMapper;
using WebApiClean.Application.Models.Sample;
using WebApiClean.Domain.Services.Interfaces.Mapping;
using MediatR;
using WebApiClean.Common.Extensions;

namespace WebApiClean.Application.Samples.Queries.Sample
{
    public class SampleQuery : IRequest<string>, IHaveCustomMapping
    {
        public string Echo { get; set; }

        public void CreateMappings(Profile configuration)
        {
            configuration.CreateMap<GetAllSampleRequest, SampleQuery>()
                .ForMember(
                    o => o.Echo,
                    opt => opt.MapFrom(o => o.Data.Return(v => v.Value)));
        }
    }
}
