using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace WebApiClean.Common.Extensions
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class MonadExtensions
    {
        public static TResult With<TInput, TResult>(this TInput o, [NotNull] Func<TInput, TResult> evaluator)
        {
            if (o == null)
                return default;

            return evaluator(o);
        }

        public static TResult Return<TInput, TResult>(this TInput o, [NotNull] Func<TInput, TResult> evaluator)
        {
            return With(o, evaluator);
        }

        public static TResult Return<TInput, TResult>(this TInput o, [NotNull] Func<TInput, TResult> evaluator, TResult failureValue)
        {
            if (o == null)
                return failureValue;

            return evaluator(o);
        }

        public static TInput If<TInput>(this TInput o, [NotNull] Func<TInput, bool> evaluator)
        {
            if (o == null)
                return default;

            return evaluator(o) ? o : default;
        }

        public static TInput Unless<TInput>(this TInput o, [NotNull] Func<TInput, bool> evaluator)
        {
            if (o == null)
                return default;

            return evaluator(o) ? default : o;
        }

        public static TInput Do<TInput>(this TInput o, [NotNull] Action<TInput> action)
        {
            if (o == null)
                return default;

            action(o);

            return o;
        }
    }
}
