using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EsiNet;
using EsiNet.Fragments;
using EsiNet.Fragments.Choose;
using EsiNet.Pipeline;
using SharpTestsEx;
using Tests.Helpers;
using Xunit;

namespace Tests
{
    public class EsiFragmentExecutorTests
    {
        [Fact]
        public async Task Should_throw_when_no_matching_executor()
        {
            var executors = new Dictionary<Type, Func<IEsiFragment, EsiExecutionContext, Task<IEnumerable<string>>>>();
            var fragmentExecutor = new EsiFragmentExecutor(executors, new PipelineContainer().GetInstance);

            var fragment = new FakeFragment();
            // ReSharper disable once PossibleNullReferenceException
            var exception = await Record.ExceptionAsync(() => fragmentExecutor.Execute(fragment, EmptyExecutionContext()));

            exception.Should().Be.InstanceOf<NotSupportedException>();
        }

        [Fact]
        public async Task Should_run_executor_for_fragment()
        {
            var executors = new Dictionary<Type, Func<IEsiFragment, EsiExecutionContext, Task<IEnumerable<string>>>>
            {
                [typeof(FakeFragment)] = (f, ec) => Task.FromResult<IEnumerable<string>>(new[] {"fake", "content"})
            };
            var fragmentExecutor = new EsiFragmentExecutor(executors, new PipelineContainer().GetInstance);

            var fragment = new FakeFragment();
            var result = await fragmentExecutor.Execute(fragment, EmptyExecutionContext());

            result.Should().Have.SameSequenceAs("fake", "content");
        }

        [Fact]
        public async Task Should_run_pipeline_when_executing()
        {
            var textExecutor = new EsiTextFragmentExecutor();
            var executors = new Dictionary<Type, Func<IEsiFragment, EsiExecutionContext, Task<IEnumerable<string>>>>
            {
                [typeof(EsiTextFragment)] = (f, ec) => textExecutor.Execute((EsiTextFragment) f, ec)
            };

            var resolver = new PipelineContainer();
            resolver.Add(new FakeTextPipeline());

            var fragmentExecutor = new EsiFragmentExecutor(executors, resolver.GetInstance);

            var fragment = new EsiTextFragment("body");
            var result = await fragmentExecutor.Execute(fragment, EmptyExecutionContext());

            result.Should().Have.SameSequenceAs("pre", "<body>", "post");
        }

        private static EsiExecutionContext EmptyExecutionContext()
        {
            return new EsiExecutionContext(
                new Dictionary<string, IReadOnlyCollection<string>>(), new Dictionary<string, IVariableValueResolver>());
        }
    }

    public class FakeFragment : IEsiFragment
    {
    }

    public class FakeTextPipeline : IFragmentExecutePipeline<EsiTextFragment>
    {
        public async Task<IEnumerable<string>> Handle(
            EsiTextFragment fragment, EsiExecutionContext executionContext, ExecuteDelegate<EsiTextFragment> next)
        {
            var result = await next(new EsiTextFragment($"<{fragment.Body}>"), executionContext);
            var pre = new[] {"pre"};
            var post = new[] {"post"};
            return pre.Concat(result).Concat(post);
        }
    }
}