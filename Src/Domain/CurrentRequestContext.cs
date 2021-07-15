using Serilog;
using System.Threading;

namespace WebApiClean.Domain
{
    public class CurrentRequestContext
    {
        private static readonly object _syncRoot = new object();
        private static readonly AsyncLocal<CurrentRequestContext> _requestContext = new AsyncLocal<CurrentRequestContext>();

        private CurrentRequestContext()
        {
        }

        public static CurrentRequestContext Current
        {
            get
            {
                lock (_syncRoot)
                {
                    var context = _requestContext.Value;
                    if (context != null)
                        return context;

                    context = new CurrentRequestContext();
                    _requestContext.Value = context;

                    return context;
                }
            }
        }

        public static CurrentRequestContext CreateNewContext()
        {
            lock (_syncRoot)
            {
                var context = new CurrentRequestContext();
                _requestContext.Value = context;

                return context;
            }
        }

        public string CorrelationId { get; set; }
        public ILogger Logger { get; set; }
    }
}
