using System.Collections.Generic;

namespace EsiNet
{
    public class EsiExecutionContext
    {
        public EsiExecutionContext(
            IReadOnlyDictionary<string, IReadOnlyCollection<string>> requestHeaders,
            IReadOnlyDictionary<string, string> variables)
        {
            RequestHeaders = requestHeaders;
            Variables = variables;
        }

        public IReadOnlyDictionary<string, IReadOnlyCollection<string>> RequestHeaders { get; }
        public IReadOnlyDictionary<string, string> Variables { get; }
    }
}