using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EsiNet;
using EsiNet.Http;
using EsiNet.Logging;
using EsiNet.Pipeline;
using FakeItEasy;
using SharpTestsEx;
using Tests.Helpers;
using Xunit;

namespace Tests.Http
{
    public class HttpLoaderTests
    {
        [Fact]
        public async Task Should_return_content_on_successful_request()
        {
            var client = new FakeHttpMessageHandler()
                .Configure(new Uri("http://host/path"), HttpStatusCode.OK, "content")
                .ToClient();
            var loader = CreateHttpLoader(client);

            var response = await loader.Get(new Uri("http://host/path"), EmptyExecutionContext());
            var content = await response.Content.ReadAsStringAsync();

            content.Should().Be.EqualTo("content");
        }

        [Theory]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.Unauthorized)]
        public async Task Should_throw_exception_on_failed_request(HttpStatusCode statusCode)
        {
            var client = new FakeHttpMessageHandler()
                .Configure(new Uri("http://host/path"), statusCode, "")
                .ToClient();
            var loader = CreateHttpLoader(client);

            // ReSharper disable once PossibleNullReferenceException
            var exception =
                await Record.ExceptionAsync(() => loader.Get(new Uri("http://host/path"), EmptyExecutionContext()));

            exception.Should().Be.InstanceOf<HttpRequestException>();
        }

        [Theory]
        [InlineData(HttpStatusCode.InternalServerError)]
        [InlineData(HttpStatusCode.ServiceUnavailable)]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.Unauthorized)]
        public async Task Should_log_exception_on_failed_request(HttpStatusCode statusCode)
        {
            var client = new FakeHttpMessageHandler()
                .Configure(new Uri("http://host/path"), statusCode, "")
                .ToClient();
            var log = A.Fake<Log>();
            var loader = CreateHttpLoader(client, log: log);

            // ReSharper disable once PossibleNullReferenceException
            var exception =
                await Record.ExceptionAsync(() => loader.Get(new Uri("http://host/path"), EmptyExecutionContext()));

            A.CallTo(() => log(LogLevel.Error, exception, A<Func<string>>._)).MustHaveHappened();
        }

        [Fact]
        public async Task Should_throw_exception_on_timeout()
        {
            var client = new FakeHttpMessageHandler()
                .Configure(new Uri("http://host/path"), _ => throw new TaskCanceledException())
                .ToClient();
            var loader = CreateHttpLoader(client);

            // ReSharper disable once PossibleNullReferenceException
            var exception =
                await Record.ExceptionAsync(() => loader.Get(new Uri("http://host/path"), EmptyExecutionContext()));

            exception.Should().Be.InstanceOf<TaskCanceledException>();
        }

        [Fact]
        public async Task Should_send_esi_header()
        {
            HttpRequestMessage request = null;
            var client = new FakeHttpMessageHandler()
                .Configure(new Uri("http://host/path"), r =>
                {
                    request = r;
                    return new HttpResponseMessage();
                })
                .ToClient();
            var loader = CreateHttpLoader(client);

            await loader.Get(new Uri("http://host/path"), EmptyExecutionContext());

            request.Headers.TryGetValues("X-Esi", out var headerValues).Should().Be.True();
            headerValues.Should().Have.SameSequenceAs("true");
        }

        [Fact]
        public async Task Should_forward_headers()
        {
            HttpRequestMessage request = null;
            var client = new FakeHttpMessageHandler()
                .Configure(new Uri("http://host/path"), r =>
                {
                    request = r;
                    return new HttpResponseMessage();
                })
                .ToClient();
            var loader = CreateHttpLoader(client);

            var executionContext = new EsiExecutionContext(
                new Dictionary<string, IReadOnlyCollection<string>>
                {
                    ["Accept"] = new[] {"text/html", "application/xhtml+xml"},
                    ["Cookie"] = new[] {"a=1; b=2"},
                    ["Connection"] = new[] {"Keep-Alive"}
                },
                new Dictionary<string, string>());
            await loader.Get(new Uri("http://host/path"), executionContext);

            request.Headers.TryGetValues("Accept", out var acceptValues).Should().Be.True();
            acceptValues.Should().Have.SameSequenceAs("text/html", "application/xhtml+xml");
            request.Headers.TryGetValues("Cookie", out var cookieValues).Should().Be.True();
            cookieValues.Should().Have.SameSequenceAs("a=1; b=2");
            request.Headers.Contains("Connection").Should().Be.False();
        }

        [Fact]
        public async Task Should_run_pipeline_for_request()
        {
            var log = new List<string>();
            var client = new FakeHttpMessageHandler()
                .Configure(new Uri("http://host/path"), r =>
                {
                    log.Add("request");
                    return new HttpResponseMessage();
                })
                .ToClient();
            var pipeline = new LoggingHttpLoaderPipeline(s => log.Add(s));
            var loader = CreateHttpLoader(client, new[] {pipeline});

            await loader.Get(new Uri("http://host/path"), EmptyExecutionContext());

            log.Should().Have.SameSequenceAs("pipeline before", "request", "pipeline after");
        }

        private static EsiExecutionContext EmptyExecutionContext()
        {
            return new EsiExecutionContext(
                new Dictionary<string, IReadOnlyCollection<string>>(), new Dictionary<string, string>());
        }

        private static HttpLoader CreateHttpLoader(HttpClient client,
            IEnumerable<IHttpLoaderPipeline> pipelines = null,
            HttpRequestMessageFactory httpRequestMessageFactory = null,
            Log log = null)
        {
            return new HttpLoader(
                uri => client,
                httpRequestMessageFactory ?? DefaultHttpRequestMessageFactory.Create,
                pipelines ?? Array.Empty<IHttpLoaderPipeline>(),
                log ?? ((level, exception, message) => { }));
        }
    }

    public class LoggingHttpLoaderPipeline : IHttpLoaderPipeline
    {
        private readonly Action<string> _log;

        public LoggingHttpLoaderPipeline(Action<string> log)
        {
            _log = log;
        }

        public async Task<HttpResponseMessage> Handle(Uri uri, EsiExecutionContext executionContext, HttpLoadDelegate next)
        {
            _log("pipeline before");
            var response = await next(uri, executionContext);
            _log("pipeline after");
            return response;
        }
    }
}