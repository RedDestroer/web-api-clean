using WebApiClean.Domain.Exceptions;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace WebApiClean.Application.Common.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public sealed class ValidationException : DomainException
    {
        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Failures = new Dictionary<string, string[]>();

            Data[nameof(Failures)] = Failures;
        }

        public ValidationException(List<ValidationFailure> failures)
            : this()
        {
            var propertyNames = failures
                .Select(e => e.PropertyName)
                .Distinct();

            foreach (var propertyName in propertyNames)
            {
                var propertyFailures = failures
                    .Where(e => e.PropertyName == propertyName)
                    .Select(e => e.ErrorMessage)
                    .ToArray();

                Failures.Add(propertyName, propertyFailures);
            }

            Data[nameof(Failures)] = Failures;
        }

        public IDictionary<string, string[]> Failures { get; }
    }
}
