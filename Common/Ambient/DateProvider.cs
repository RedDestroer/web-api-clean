using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace WebApiClean.Common.Ambient
{
    [ExcludeFromCodeCoverage]
    public abstract class DateProvider
    {
        private static DateProvider _current;

        static DateProvider()
        {
            ResetToDefault();
        }

        public static DateProvider Current
        {
            get => _current;
            set => _current = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static void ResetToDefault() => Current = new DefaultDateProvider();

        public abstract DateTime Now();
        public abstract DateTime UtcNow();
        public abstract DateTime Today();

        private class DefaultDateProvider : DateProvider
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override DateTime Now() => DateTime.Now;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override DateTime UtcNow() => DateTime.UtcNow;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override DateTime Today() => DateTime.Today;
        }
    }
}
