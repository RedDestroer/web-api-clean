using WebApiClean.Domain.Services.Interfaces.Mapping;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace WebApiClean.Infrastructure.Mapping
{
    [ExcludeFromCodeCoverage]
    public static class MapperProfileHelper
    {
        public static IList<Map> LoadStandardMappings([NotNull] Assembly rootAssembly)
        {
            var types = rootAssembly.GetExportedTypes();

            var mapsFrom = (
                from type in types
                from instance in type.GetInterfaces()
                where
                    instance.IsGenericType && instance.GetGenericTypeDefinition() == typeof(IMapFrom<>) &&
                    !type.IsAbstract &&
                    !type.IsInterface
                select new Map
                {
                    Source = type.GetInterfaces().First().GetGenericArguments().First(),
                    Destination = type,
                }).ToList();

            return mapsFrom;
        }

        public static IList<Map> LoadNonStandardMappings([NotNull] Assembly rootAssembly)
        {
            var types = rootAssembly.GetExportedTypes();

            var mapsTo = (from type in types
                          from i in type.GetInterfaces()
                          where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapTo<>) &&
                                !type.IsAbstract &&
                                !type.IsInterface
                          select new Map
                          {
                              Source = type,
                              Destination = type.GetInterfaces().First().GetGenericArguments().First(),
                          }).ToList();

            return mapsTo;
        }

        public static IList<IHaveCustomMapping> LoadCustomMappings([NotNull] Assembly rootAssembly)
        {
            var types = rootAssembly.GetExportedTypes();

            var mapsFrom = (
                from type in types
                let defaultConstructor = type.GetConstructor(Type.EmptyTypes)
                where
                    typeof(IHaveCustomMapping).IsAssignableFrom(type) &&
                    !type.IsAbstract &&
                    !type.IsInterface &&
                    defaultConstructor != null
                select (IHaveCustomMapping)Activator.CreateInstance(type)).ToList();

            return mapsFrom;
        }
    }
}
