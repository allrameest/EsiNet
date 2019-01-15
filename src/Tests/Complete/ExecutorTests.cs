using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using EsiNet;
using EsiNet.AspNetCore;
using EsiNet.Caching;
using EsiNet.Fragments;
using EsiNet.Http;
using EsiNet.Logging;
using EsiNet.Pipeline;
using FakeItEasy;
using SharpTestsEx;
using Tests.Helpers;
using Xunit;

namespace Tests.Complete
{
    public class ExecutorTests
    {
        [Fact]
        public async Task Execute_IncludeFragment_ReturnContentFromRequest()
        {
            var fakeStaticHttpLoader = new FakeStaticHttpLoader(
                new Dictionary<string, (string, int?)>
                {
                    ["http://host/fragment"] = ("Included", null)
                });
            var fragment = new EsiIncludeFragment(new Uri("http://host/fragment"));

            var content = await Execute(fragment, fakeStaticHttpLoader);

            content.Should().Be.EqualTo("Included");
        }

        [Fact]
        public async Task Execute_CompositeFragment_ReturnConcatenated()
        {
            var httpLoader = new FakeStaticHttpLoader(new Dictionary<string, (string, int?)>());
            var fragment = new EsiCompositeFragment(new IEsiFragment[]
            {
                new EsiTextFragment("A"),
                new EsiIgnoreFragment(),
                new EsiTextFragment("B")
            });

            var content = await Execute(fragment, httpLoader);

            content.Should().Be.EqualTo("AB");
        }

        [Fact]
        public async Task Execute_TryFragmentWhereAttemptFails_ReturnExceptFragmentAndLog()
        {
            var exception = new HttpRequestException();
            var httpLoader = A.Fake<IHttpLoader>();
            var log = A.Fake<Log>();
            A.CallTo(() => httpLoader.Get(A<Uri>._, A<EsiExecutionContext>._)).Throws(exception);
            var fragment = new EsiTryFragment(
                new EsiIncludeFragment(new Uri("http://host/fragment")),
                new EsiTextFragment("Fallback"));

            var content = await Execute(fragment, httpLoader, log);

            content.Should().Be.EqualTo("Fallback");
            A.CallTo(() => log(LogLevel.Error, exception, A<Func<string>>._)).MustHaveHappened();
        }

        private static async Task<string> Execute(
            IEsiFragment fragment, IHttpLoader httpLoader, Log log = null)
        {
            log = log ?? ((level, exception, message) => { });
            var executor = EsiExecutorFactory.Create(
                new NullEsiFragmentCache(),
                httpLoader,
                EsiParserFactory.Create(Array.Empty<IFragmentParsePipeline>(), url => new Uri(url)),
                log,
                new PipelineContainer().GetInstance);
            var content = await executor.Execute(fragment, EmptyExecutionContext());

            return string.Concat(content);
        }

        private static EsiExecutionContext EmptyExecutionContext()
        {
            return new EsiExecutionContext(new Dictionary<string, IReadOnlyCollection<string>>());
        }
    }
}