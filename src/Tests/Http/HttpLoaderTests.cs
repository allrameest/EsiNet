using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
        private readonly Log _nullLog = (level, exception, message) => {};

        [Fact]
        public async Task Should_return_content_on_successful_request()
        {
            var client = new FakeHttpMessageHandler()
                .Configure(new Uri("http://host/path"), HttpStatusCode.OK, "content")
                .ToClient();
            var loader = new HttpLoader(uri => client, Array.Empty<IHttpLoaderPipeline>(), _nullLog);

            var response = await loader.Get(new Uri("http://host/path"));
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
            var loader = new HttpLoader(uri => client, Array.Empty<IHttpLoaderPipeline>(), _nullLog);

            // ReSharper disable once PossibleNullReferenceException
            var exception = await Record.ExceptionAsync(() => loader.Get(new Uri("http://host/path")));

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
            var loader = new HttpLoader(uri => client, Array.Empty<IHttpLoaderPipeline>(), log);

            // ReSharper disable once PossibleNullReferenceException
            var exception = await Record.ExceptionAsync(() => loader.Get(new Uri("http://host/path")));

            A.CallTo(() => log(LogLevel.Error, exception, A<Func<string>>._)).MustHaveHappened();
        }

        [Fact]
        public async Task Should_throw_exception_on_timeout()
        {
            var client = new FakeHttpMessageHandler()
                .Configure(new Uri("http://host/path"), _ => throw new TaskCanceledException())
                .ToClient();
            var loader = new HttpLoader(uri => client, Array.Empty<IHttpLoaderPipeline>(), _nullLog);

            // ReSharper disable once PossibleNullReferenceException
            var exception = await Record.ExceptionAsync(() => loader.Get(new Uri("http://host/path")));

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
            var loader = new HttpLoader(uri => client, Array.Empty<IHttpLoaderPipeline>(), _nullLog);

            await loader.Get(new Uri("http://host/path"));

            request.Headers.TryGetValues("X-Esi", out var headerValues).Should().Be.True();
            headerValues.Should().Have.SameSequenceAs("true");
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
            var loader = new HttpLoader(uri => client, new[] {pipeline}, _nullLog);

            await loader.Get(new Uri("http://host/path"));

            log.Should().Have.SameSequenceAs("pipeline before", "request", "pipeline after");
        }
    }

    public class LoggingHttpLoaderPipeline : IHttpLoaderPipeline
    {
        private readonly Action<string> _log;

        public LoggingHttpLoaderPipeline(Action<string> log)
        {
            _log = log;
        }

        public async Task<HttpResponseMessage> Handle(Uri uri, HttpLoadDelegate next)
        {
            _log("pipeline before");
            var response = await next(uri);
            _log("pipeline after");
            return response;
        }
    }
}