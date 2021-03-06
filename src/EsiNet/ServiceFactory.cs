﻿using System;
using System.Collections.Generic;

namespace EsiNet
{
    public delegate object ServiceFactory(Type serviceType);

    public static class ServiceFactoryExtensions
    {
        public static T GetInstance<T>(this ServiceFactory factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            return (T) factory(typeof(T));
        }

        public static IEnumerable<T> GetInstances<T>(this ServiceFactory factory)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            return (IEnumerable<T>) factory(typeof(IEnumerable<T>));
        }
    }
}