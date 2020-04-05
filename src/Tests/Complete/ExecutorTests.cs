using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using EsiNet;
using EsiNet.AspNetCore;
using EsiNet.Caching;
using EsiNet.Expressions;
using EsiNet.Fragments;
using EsiNet.Fragments.Choose;
using EsiNet.Fragments.Composite;
using EsiNet.Fragments.Ignore;
using EsiNet.Fragments.Include;
using EsiNet.Fragments.Text;
using EsiNet.Fragments.Try;
using EsiNet.Fragments.Vars;
using EsiNet.Http;
using EsiNet.Logging;
using EsiNet.Pipeline;
using FakeItEasy;
using Microsoft.Extensions.Caching.Memory;
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
            var fragment = EsiIncludeFragmentFactory.Create("http://host/fragment");

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
                EsiIncludeFragmentFactory.Create("http://host/fragment"),
                new EsiTextFragment("Fallback"));

            var content = await Execute(fragment, httpLoader, log);

            content.Should().Be.EqualTo("Fallback");
            A.CallTo(() => log(LogLevel.Error, exception, A<Func<string>>._)).MustHaveHappened();
        }

        [Fact]
        public async Task Execute_IncludeFragment_ReturnFromCacheSecondTime()
        {
            var httpLoader = A.Fake<IHttpLoader>();
            A.CallTo(() => httpLoader.Get(A<Uri>._, A<EsiExecutionContext>._))
                .Returns(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("response content"),
                    Headers =
                    {
                        CacheControl = new CacheControlHeaderValue
                        {
                            Public = true,
                            MaxAge = TimeSpan.FromMinutes(1)
                        }
                    }
                });
            var uri = new Uri("http://host/fragment");
            var fragment = EsiIncludeFragmentFactory.Create(uri.ToString());
            var executor = CreateExecutor(httpLoader);
            var executionContext = CreateExecutionContext();

            await executor.Execute(fragment, executionContext);
            var content = await executor.Execute(fragment, executionContext);

            content.Should().Have.SameSequenceAs("response content");
            A.CallTo(() => httpLoader.Get(uri, A<EsiExecutionContext>._)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public async Task Execute_IncludeFragmentWithVary_CacheUsingVary()
        {
            var httpLoader = A.Fake<IHttpLoader>();
            A.CallTo(() => httpLoader.Get(A<Uri>._, A<EsiExecutionContext>._))
                .Returns(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("response content"),
                    Headers =
                    {
                        CacheControl = new CacheControlHeaderValue
                        {
                            Public = true,
                            MaxAge = TimeSpan.FromMinutes(1)
                        },
                        Vary = {"Accept"}
                    }
                });
            var uri = new Uri("http://host/fragment");
            var fragment = EsiIncludeFragmentFactory.Create(uri.ToString());
            var executor = CreateExecutor(httpLoader);

            await executor.Execute(fragment, CreateExecutionContext(KeyValuePair.Create("Accept", "application/xml")));
            await executor.Execute(fragment, CreateExecutionContext(KeyValuePair.Create("Accept", "application/xml")));
            await executor.Execute(fragment, CreateExecutionContext(KeyValuePair.Create("Accept", "application/json")));
            await executor.Execute(fragment, CreateExecutionContext(KeyValuePair.Create("Accept", "application/json")));

            A.CallTo(() => httpLoader.Get(uri, A<EsiExecutionContext>._)).MustHaveHappenedTwiceExactly();
        }

        [Theory]
        [InlineData("example.com", "example")]
        [InlineData("whatever", "not localhost")]
        [InlineData("localhost", "other")]
        public async Task Execute_ChooseFragment_ReturnResult(string host, string expected)
        {
            var httpLoader = A.Fake<IHttpLoader>();
            var fragment = new EsiChooseFragment(
                new[]
                {
                    new EsiWhenFragment(
                        new ComparisonExpression(
                            new SimpleVariableExpression("HTTP_HOST"),
                            new ConstantExpression("example.com"),
                            ComparisonOperator.Equal, BooleanOperator.And),
                        new EsiTextFragment("example")),
                    new EsiWhenFragment(
                        new ComparisonExpression(
                            new SimpleVariableExpression("HTTP_HOST"),
                            new ConstantExpression("localhost"),
                            ComparisonOperator.NotEqual, BooleanOperator.And),
                        new EsiTextFragment("not localhost"))
                },
                new EsiTextFragment("other"));
            var executionContext = new EsiExecutionContext(
                new Dictionary<string, IReadOnlyCollection<string>>(),
                new Dictionary<string, IVariableValueResolver>
                {
                    ["HTTP_HOST"] = new SimpleVariableValueResolver(new Lazy<string>(host))
                });

            var executor = CreateExecutor(httpLoader);
            var content = await executor.Execute(fragment, executionContext);

            string.Concat(content).Should().Be.EqualTo(expected);
        }

        [Fact]
        public async Task Execute_VarsFragment_ReturnResult()
        {
            var httpLoader = new FakeStaticHttpLoader(new Dictionary<string, (string, int?)>());
            var fragment = new EsiVarsFragment(new VariableString(new object[]
            {
                "Host: ",
                new SimpleVariableExpression("HTTP_HOST")
            }));
            var executionContext = new EsiExecutionContext(
                new Dictionary<string, IReadOnlyCollection<string>>(),
                new Dictionary<string, IVariableValueResolver>
                {
                    ["HTTP_HOST"] = new SimpleVariableValueResolver(new Lazy<string>("localhost"))
                });

            var executor = CreateExecutor(httpLoader);
            var content = await executor.Execute(fragment, executionContext);

            string.Concat(content).Should().Be.EqualTo("Host: localhost");
        }

        [Fact]
        public async Task Execute_IncludeFragmentWithVariables_ReturnContentFromRequest()
        {
            var httpLoader = new FakeStaticHttpLoader(
                new Dictionary<string, (string, int?)>
                {
                    ["http://host/ninja"] = ("Included", null)
                });
            var fragment = new EsiIncludeFragment(new VariableString(new object[]
            {
                "http://host/",
                new DictionaryVariableExpression("HTTP_COOKIE", "path")
            }));
            var executionContext = new EsiExecutionContext(
                new Dictionary<string, IReadOnlyCollection<string>>(),
                new Dictionary<string, IVariableValueResolver>
                {
                    ["HTTP_COOKIE"] = new DictionaryVariableValueResolver(
                        new Lazy<IReadOnlyDictionary<string, string>>(
                            () => new Dictionary<string, string>
                            {
                                ["path"] = "ninja"
                            }))
                });

            var executor = CreateExecutor(httpLoader);
            var content = await executor.Execute(fragment, executionContext);

            string.Concat(content).Should().Be.EqualTo("Included");
        }

        private static async Task<string> Execute(
            IEsiFragment fragment, IHttpLoader httpLoader, Log log = null)
        {
            var executor = CreateExecutor(httpLoader, log);
            var content = await executor.Execute(fragment, CreateExecutionContext());

            return string.Concat(content);
        }

        private static EsiFragmentExecutor CreateExecutor(IHttpLoader httpLoader, Log log = null)
        {
            return EsiExecutorFactory.Create(
                new EsiFragmentCacheFacade(
                    new MemoryEsiFragmentCache(new MemoryCache(new MemoryCacheOptions())),
                    new MemoryVaryHeaderStore()),
                httpLoader,
                EsiParserFactory.Create(Array.Empty<IFragmentParsePipeline>()),
                log ?? ((level, exception, message) => { }),
                new PipelineContainer().GetInstance,
                url => new Uri(url));
        }

        private static EsiExecutionContext CreateExecutionContext(params KeyValuePair<string, string>[] requestHeaders)
        {
            return new EsiExecutionContext(
                requestHeaders
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => (IReadOnlyCollection<string>) new[] {kvp.Value},
                        StringComparer.OrdinalIgnoreCase),
                new Dictionary<string, IVariableValueResolver>());
        }
    }
}