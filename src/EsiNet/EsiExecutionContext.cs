using System.Collections.Generic;
using EsiNet.Fragments.Choose;

namespace EsiNet
{
    public class EsiExecutionContext
    {
        public EsiExecutionContext(
            IReadOnlyDictionary<string, IReadOnlyCollection<string>> requestHeaders,
            IReadOnlyDictionary<string, IVariableValueResolver> variables)
        {
            RequestHeaders = requestHeaders;
            Variables = variables;
        }

        public IReadOnlyDictionary<string, IReadOnlyCollection<string>> RequestHeaders { get; }
        public IReadOnlyDictionary<string, IVariableValueResolver> Variables { get; }
    }
}