using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AutoMapper;

namespace WebApiClean.Infrastructure.Mapping
{
    [ExcludeFromCodeCoverage]
    public class AutoMapperProfile : Profile
    {
        private readonly Assembly[] _assemblies;

        private AutoMapperProfile(Assembly[] assemblies)
        {
            _assemblies = assemblies;

            LoadStandardMappings();
            LoadNonStandardMappings();
            LoadCustomMappings();
            LoadConverters();
        }

        public static Profile Initialize(params Assembly[] assemblies) =>
            new AutoMapperProfile(assemblies ?? new[]
            {
                Assembly.GetExecutingAssembly(),
            });

        /// <inheritdoc />
        public override string ProfileName => typeof(AutoMapperProfile).FullName;

        private static void LoadConverters()
        {
            // AddOrUpdate converters
        }

        private void LoadStandardMappings()
        {
            foreach (var assembly in _assemblies)
            {
                var mapsFrom = MapperProfileHelper.LoadStandardMappings(assembly);
                foreach (var map in mapsFrom)
                {
                    CreateMap(map.Source, map.Destination).ReverseMap();
                }
            }
        }

        private void LoadNonStandardMappings()
        {
            foreach (var assembly in _assemblies)
            {
                var mapsTo = MapperProfileHelper.LoadNonStandardMappings(assembly);
                foreach (var map in mapsTo)
                {
                    CreateMap(map.Source, map.Destination);
                }
            }
        }

        private void LoadCustomMappings()
        {
            foreach (var assembly in _assemblies)
            {
                var customMaps = MapperProfileHelper.LoadCustomMappings(assembly);
                foreach (var map in customMaps)
                {
                    map.CreateMappings(this);
                }
            }
        }
    }
}
