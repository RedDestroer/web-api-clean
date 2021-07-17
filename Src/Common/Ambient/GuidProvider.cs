using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace WebApiClean.Common.Ambient
{
    [ExcludeFromCodeCoverage]
    public abstract class GuidProvider
    {
        private static GuidProvider _current;

        static GuidProvider()
        {
            ResetToDefault();
        }

        public static GuidProvider Current
        {
            get => _current;
            set => _current = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static void ResetToDefault() => Current = new DefaultGuidProvider();

        public abstract Guid NewGuid();

        private class DefaultGuidProvider : GuidProvider
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override Guid NewGuid() => Guid.NewGuid();
        }
    }
}
