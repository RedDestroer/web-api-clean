using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace WebApiClean.Domain.Common
{
    // Source: https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/microservice-ddd-cqrs-patterns/implement-value-objects
    [ExcludeFromCodeCoverage]
    public abstract class ValueObject : ICloneable
    {
        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            return ValueObjectEquals(obj as ValueObject);
        }

        public object Clone() => CloneInner();

        public override int GetHashCode() =>
            GetAtomicValues()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);

        protected static bool EqualOperator(ValueObject left, ValueObject right)
        {
            if (left is null ^ right is null)
                return false;

            return left?.Equals(right) != false;
        }

        protected static bool NotEqualOperator(ValueObject left, ValueObject right) =>
            !EqualOperator(left, right);

        protected abstract IEnumerable<object> GetAtomicValues();

        protected bool ValueObjectEquals<T>(T other) where T : ValueObject
        {
            if (other == null)
                return false;

            return EnumerableEquals(GetAtomicValues(), other.GetAtomicValues());
        }

        protected bool EnumerableEquals(IEnumerable left, IEnumerable right)
        {
            using var thisValues = left.OfType<object>().GetEnumerator();
            using var otherValues = right.OfType<object>().GetEnumerator();

            while (thisValues.MoveNext() && otherValues.MoveNext())
            {
                if (thisValues.Current is null ^ otherValues.Current is null)
                    return false;

                if (thisValues.Current != null)
                {
                    if (thisValues.Current is string)
                    {
                        if (!thisValues.Current.Equals(otherValues.Current))
                            return false;
                    }
                    else if (thisValues.Current is IEnumerable enumerable)
                    {
                        if (!EnumerableEquals(enumerable, (IEnumerable)otherValues.Current))
                            return false;
                    }
                    else if (!thisValues.Current.Equals(otherValues.Current))
                    {
                        return false;
                    }
                }
            }

            return !thisValues.MoveNext() && !otherValues.MoveNext();
        }

        protected virtual object CloneInner()
        {
            var type = GetType();
            var constructors = type.GetConstructors();

            if (constructors.Length != 1)
                throw new InvalidOperationException("If you don't have single public constructor, you have to override CloneInner.");

            object[] values = GetAtomicValues()
                .Select(o =>
                {
                    if (o is ICloneable cloneable)
                        return cloneable.Clone();

                    return o;
                })
                .ToArray();

            return Activator.CreateInstance(type, values);
        }
    }
}
