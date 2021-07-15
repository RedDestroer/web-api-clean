using AutoMapper;

namespace WebApiClean.Domain.Services.Interfaces.Mapping
{
    public interface IHaveCustomMapping
    {
        void CreateMappings(Profile configuration);
    }
}
