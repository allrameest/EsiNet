using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsiNet.Caching;

namespace EsiNet.AspNetCore
{
    public static class EsiExecutorFactory
    {
        public static EsiFragmentExecutor Create(IEsiFragmentCache cache, IHttpLoader httpLoader, EsiBodyParser parser)
        {
            var executors = new Dictionary<Type, Func<IEsiFragment, Task<string>>>();

            var fragmentExecutor = new EsiFragmentExecutor(executors);
            var includeExecutor = new EsiIncludeFragmentExecutor(cache, httpLoader, parser, fragmentExecutor);
            var ignoreExecutor = new EsiIgnoreFragmentExecutor();
            var textExecutor = new EsiTextFragmentExecutor();
            var compositeExecutor = new EsiCompositeFragmentExecutor(fragmentExecutor);

            executors.Add(typeof(EsiIncludeFragment), f => includeExecutor.Execute((EsiIncludeFragment) f));
            executors.Add(typeof(EsiIgnoreFragment), f => ignoreExecutor.Execute((EsiIgnoreFragment) f));
            executors.Add(typeof(EsiTextFragment), f => textExecutor.Execute((EsiTextFragment) f));
            executors.Add(typeof(EsiCompositeFragment), f => compositeExecutor.Execute((EsiCompositeFragment) f));

            return fragmentExecutor;
        }
    }
}