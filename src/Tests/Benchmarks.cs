using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EsiNet;
using EsiNet.AspNetCore;
using EsiNet.Caching;
using EsiNet.Http;
using EsiNet.Pipeline;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Xunit;
using Xunit.Abstractions;
using CacheControlHeaderValue = System.Net.Http.Headers.CacheControlHeaderValue;

namespace Tests
{
    public class Benchmarks
    {
        private readonly ITestOutputHelper _output;

        public Benchmarks(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task SimpleCached()
        {
            var urlContentMap = new Dictionary<string, (string, int?)>
            {
                ["/"] = (@"...<esi:include src=""/header""/>...", 60),
                ["/header"] = ("HEADER", 3600)
            };

            await BenchmarkEsi(urlContentMap);
        }

        [Fact]
        public async Task Nested3LevelsCached()
        {
            var urlContentMap = Create3LevelsOfUrls(5, 5, 5);

            await BenchmarkEsi(urlContentMap);
        }

        [Fact]
        public async Task Nested3LevelsNoCache()
        {
            var urlContentMap = Create3LevelsOfUrls(5, 5, 5, null);

            await BenchmarkEsi(urlContentMap);
        }

        private async Task<string> BenchmarkEsi(Dictionary<string, (string, int?)> urlContentMap)
        {
            var cache = CreateCache();
            var parser = CreateParser();
            var executor = CreateExecutor(cache, urlContentMap, parser);

            return await Benchmark(async () =>
            {
                var (rootContent, rootCache) = urlContentMap["/"];
                var cacheControl = rootCache.HasValue
                    ? CacheControlHeaderValue.Parse($"public,max-age={rootCache.Value}")
                    : null;

                var fragment = await cache.GetOrAddFragment("/",
                    () =>
                    {
                        var esiFragment = parser.Parse(rootContent);
                        var result = (esiFragment, cacheControl);
                        return Task.FromResult(result);
                    });

                return await executor.Execute(fragment);
            });
        }

        private static IEsiFragmentCache CreateCache()
        {
            return new TwoStageEsiFragmentCache(
                new MemoryCache(
                    new MemoryCacheOptions()),
                new MemoryDistributedCache(
                    new OptionsWrapper<MemoryDistributedCacheOptions>(
                        new MemoryDistributedCacheOptions())),
                Serializer.Wire());
        }

        private static EsiBodyParser CreateParser()
        {
            return EsiParserFactory.Create(Array.Empty<IFragmentParsePipeline>());
        }

        private static EsiFragmentExecutor CreateExecutor(
            IEsiFragmentCache cache,
            Dictionary<string, (string, int?)> urlContentMap,
            EsiBodyParser parser)
        {
            return EsiExecutorFactory.Create(
                cache,
                new FakeStaticHttpLoader(urlContentMap),
                parser,
                (level, exception, message) => { }, NullPipelineFactory.Create);
        }

        private async Task<T> Benchmark<T>(Func<Task<T>> action, int count = 1000)
        {
            // Warmup
            var result = await action();

            var sw = Stopwatch.StartNew();
            for (var i = 0; i < count; i++)
            {
                await action();
            }

            sw.Stop();
            _output.WriteLine($"{Math.Round(sw.Elapsed.TotalMilliseconds, 3)} ms");

            return result;
        }

        private static Dictionary<string, (string, int?)> Create3LevelsOfUrls(int level1Count, int level2Count,
            int level3Count, int? maxAge = 60)
        {
            var urlContentMap = new Dictionary<string, (string, int?)>();

            var root = new List<string>();
            for (var i = 0; i < level1Count; i++)
            {
                var lvl1 = new List<string>();

                for (var j = 0; j < level2Count; j++)
                {
                    var lvl2 = new List<string>();

                    for (var k = 0; k < level3Count; k++)
                    {
                        var lvl3Url = $"/{i}/{j}/{k}";
                        lvl2.Add($@"<esi:include src=""{lvl3Url}""/>");
                        urlContentMap[lvl3Url] = ($"fragment({lvl3Url})", maxAge);
                    }

                    var lvl2Url = $"/{i}/{j}";
                    lvl1.Add($@"<esi:include src=""{lvl2Url}""/>");
                    urlContentMap[lvl2Url] = (string.Join("", lvl2), maxAge);
                }

                var lvl1Url = $"/{i}";
                root.Add($@"<esi:include src=""{lvl1Url}""/>");
                urlContentMap[lvl1Url] = (string.Join("#####", lvl1), maxAge);
            }

            urlContentMap["/"] = (string.Join("-----", root), maxAge);
            return urlContentMap;
        }
    }

    public class FakeStaticHttpLoader : IHttpLoader
    {
        private readonly Dictionary<string, (string, int?)> _urlContentMap;

        public FakeStaticHttpLoader(Dictionary<string, (string, int?)> urlContentMap)
        {
            _urlContentMap = urlContentMap;
        }

        public Task<HttpResponseMessage> Get(string url)
        {
            var (content, maxAge) = _urlContentMap[url];
            var response = new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent(content)};
            var cacheHeader = maxAge.HasValue ? $"public,max-age={maxAge.Value}" : "private";
            response.Headers.Add(HeaderNames.CacheControl, cacheHeader);

            return Task.FromResult(response);
        }
    }
}