using System.Collections.Generic;

namespace WebApiClean.Domain
{
    public class ResponseWithWarnings
    {
        public ResponseWithWarnings()
        {
            Warnings = new List<FailureResponse>();
        }

        public List<FailureResponse> Warnings { get; }
    }
}
