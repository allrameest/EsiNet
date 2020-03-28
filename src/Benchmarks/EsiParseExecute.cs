using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using EsiNet;
using EsiNet.AspNetCore;
using EsiNet.Caching;
using EsiNet.Caching.Serialization;
using EsiNet.Expressions;
using EsiNet.Pipeline;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Abstractions;
using CacheControlHeaderValue = System.Net.Http.Headers.CacheControlHeaderValue;

namespace Benchmarks
{
    public class EsiParseExecute
    {
        private readonly ITestOutputHelper _output;

        public EsiParseExecute(ITestOutputHelper output)
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

            return await Benchmark(async stream =>
            {
                var (rootContent, rootCache) = urlContentMap["/"];
                var cacheControl = rootCache.HasValue
                    ? CacheControlHeaderValue.Parse($"public,max-age={rootCache.Value}")
                    : null;

                var executionContext = EmptyExecutionContext();
                var fragment = await cache.GetOrAdd(new Uri("/", UriKind.Relative), 
                    executionContext,
                    () =>
                    {
                        var esiFragment = parser.Parse(rootContent);
                        var result = CacheResponse.Create(esiFragment, cacheControl, Array.Empty<string>());
                        return Task.FromResult(result);
                    });

                var content = await executor.Execute(fragment, EmptyExecutionContext());

                foreach (var part in content)
                {
                    var bytes = Encoding.UTF8.GetBytes(part);
                    await stream.WriteAsync(bytes, 0, bytes.Length);
                }
            });
        }

        private static EsiFragmentCacheFacade CreateCache()
        {
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var distributedCache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
            var serializer = Serializer.Hyperion();
            var cache = new TwoStageEsiFragmentCache(memoryCache, distributedCache, serializer);
            return new EsiFragmentCacheFacade(cache, new MemoryVaryHeaderStore());
        }

        private static EsiBodyParser CreateParser()
        {
            return EsiParserFactory.Create(
                Array.Empty<IFragmentParsePipeline>());
        }

        private static EsiFragmentExecutor CreateExecutor(
            EsiFragmentCacheFacade cache,
            Dictionary<string, (string, int?)> urlContentMap,
            EsiBodyParser parser)
        {
            return EsiExecutorFactory.Create(
                cache,
                new FakeStaticHttpLoader(urlContentMap),
                parser,
                (level, exception, message) => { },
                NullPipelineFactory.Create,
                url => new Uri(url, UriKind.RelativeOrAbsolute));
        }

        private async Task<string> Benchmark(Func<Stream, Task> action, int count = 1000)
        {
            // Warmup
            string result;
            using (var stream = new MemoryStream())
            {
                await action(stream);
                using (var streamReader = new StreamReader(stream))
                {
                    result = await streamReader.ReadToEndAsync();
                }
            }

            var sw = new Stopwatch();
            for (var i = 0; i < count; i++)
            {
                using (var stream = new MemoryStream())
                {
                    sw.Start();
                    await action(stream);
                    sw.Stop();
                }
            }

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
                        lvl2.Add($@"<esi:include src=""{lvl3Url}""/>" + new string(' ', 1000));
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

        private static EsiExecutionContext EmptyExecutionContext()
        {
            return new EsiExecutionContext(
                new Dictionary<string, IReadOnlyCollection<string>>(), new Dictionary<string, IVariableValueResolver>());
        }
    }
}