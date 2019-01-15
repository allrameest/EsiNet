using System;
using EsiNet.Fragments;
using EsiNet.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace EsiNet.AspNetCore
{
    public static class EsiNetBuilderExtensions
    {
        public static IEsiNetBuilder AddFragmentParsePipeline<TImplementation>(
            this IEsiNetBuilder builder)
            where TImplementation : class, IFragmentParsePipeline
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            builder.Services.AddSingleton<IFragmentParsePipeline, TImplementation>();
            return builder;
        }

        public static IEsiNetBuilder AddFragmentParsePipeline(
            this IEsiNetBuilder builder,
            Func<IServiceProvider, IFragmentParsePipeline> implementationFactory)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (implementationFactory == null) throw new ArgumentNullException(nameof(implementationFactory));
            builder.Services.AddSingleton(implementationFactory);
            return builder;
        }

        public static IEsiNetBuilder AddFragmentExecutePipeline<TFragment, TImplementation>(
            this IEsiNetBuilder builder)
            where TFragment : IEsiFragment
            where TImplementation : class, IFragmentExecutePipeline<TFragment>
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            builder.Services.AddSingleton<IFragmentExecutePipeline<TFragment>, TImplementation>();
            return builder;
        }

        public static IEsiNetBuilder AddFragmentExecutePipeline<TFragment>(
            this IEsiNetBuilder builder,
            Func<IServiceProvider, IFragmentExecutePipeline<TFragment>> implementationFactory)
            where TFragment : IEsiFragment
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (implementationFactory == null) throw new ArgumentNullException(nameof(implementationFactory));
            builder.Services.AddSingleton(implementationFactory);
            return builder;
        }
        public static IEsiNetBuilder AddHttpLoaderPipeline<TImplementation>(
            this IEsiNetBuilder builder)
            where TImplementation : class, IHttpLoaderPipeline
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            builder.Services.AddSingleton<IHttpLoaderPipeline, TImplementation>();
            return builder;
        }

        public static IEsiNetBuilder AddHttpLoaderPipeline(
            this IEsiNetBuilder builder,
            Func<IServiceProvider, IHttpLoaderPipeline> implementationFactory)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (implementationFactory == null) throw new ArgumentNullException(nameof(implementationFactory));
            builder.Services.AddSingleton(implementationFactory);
            return builder;
        }
    }
}