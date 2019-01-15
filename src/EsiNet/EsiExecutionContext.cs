using System.Collections.Generic;

namespace EsiNet
{
    public class EsiExecutionContext
    {
        public EsiExecutionContext(IReadOnlyDictionary<string, IReadOnlyCollection<string>> requestHeaders)
        {
            RequestHeaders = requestHeaders;
        }

        public IReadOnlyDictionary<string, IReadOnlyCollection<string>> RequestHeaders { get; }
    }
}