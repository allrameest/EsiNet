using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EsiNet.Caching;
using EsiNet.Fragments;
using EsiNet.Http;
using EsiNet.Logging;

namespace EsiNet.AspNetCore
{
    public static class EsiExecutorFactory
    {
        public static EsiFragmentExecutor Create(
            IEsiFragmentCache cache,
            IHttpLoader httpLoader,
            EsiBodyParser parser,
            Log log,
            ServiceFactory serviceFactory)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (httpLoader == null) throw new ArgumentNullException(nameof(httpLoader));
            if (parser == null) throw new ArgumentNullException(nameof(parser));
            if (log == null) throw new ArgumentNullException(nameof(log));
            if (serviceFactory == null) throw new ArgumentNullException(nameof(serviceFactory));

            var executors = new Dictionary<Type, Func<IEsiFragment, Task<IEnumerable<string>>>>();

            var fragmentExecutor = new EsiFragmentExecutor(executors, serviceFactory);
            var includeExecutor = new EsiIncludeFragmentExecutor(cache, httpLoader, parser, fragmentExecutor);
            var ignoreExecutor = new EsiIgnoreFragmentExecutor();
            var textExecutor = new EsiTextFragmentExecutor();
            var compositeExecutor = new EsiCompositeFragmentExecutor(fragmentExecutor);
            var tryExecutor = new EsiTryFragmentExecutor(fragmentExecutor, log);

            executors[typeof(EsiIncludeFragment)] = f => includeExecutor.Execute((EsiIncludeFragment) f);
            executors[typeof(EsiIgnoreFragment)] = f => ignoreExecutor.Execute((EsiIgnoreFragment) f);
            executors[typeof(EsiTextFragment)] = f => textExecutor.Execute((EsiTextFragment) f);
            executors[typeof(EsiCompositeFragment)] = f => compositeExecutor.Execute((EsiCompositeFragment) f);
            executors[typeof(EsiTryFragment)] = f => tryExecutor.Execute((EsiTryFragment) f);

            return fragmentExecutor;
        }
    }
}