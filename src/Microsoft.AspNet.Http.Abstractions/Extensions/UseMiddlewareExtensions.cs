// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Builder
{
    public static class UseMiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddleware<T>(this IApplicationBuilder builder, params object[] args)
        {
            return builder.UseMiddleware(typeof(T), args);
        }

        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder, Type middleware, params object[] args)
        {
            var applicationServices = builder.ApplicationServices;
            return builder.Use(next =>
            {
                var instance = ActivatorUtilities.CreateInstance(builder.ApplicationServices, middleware, new[] { next }.Concat(args).ToArray());
                var methodinfo = middleware.GetMethod("Invoke", BindingFlags.Instance | BindingFlags.Public);
                if (methodinfo == null)
                {
                    throw new InvalidOperationException("No public Invoke method found");
                }

                if (!typeof(Task).IsAssignableFrom(methodinfo.ReturnType))
                {
                    throw new InvalidOperationException("Invoke does not return an object of type Task");
                }

                var parameters = methodinfo.GetParameters();

                if (parameters.Length == 0 || parameters[0].ParameterType != typeof(HttpContext))
                {
                    throw new InvalidOperationException("Middleware Invoke method must take first argument of HttpContext");
                }

                if (parameters.Length == 1)
                {
                    return (RequestDelegate)methodinfo.CreateDelegate(typeof(RequestDelegate), instance);
                }

                return context =>
                {
                    var serviceProvider = context.RequestServices ?? context.ApplicationServices ?? applicationServices;
                    if (serviceProvider == null)
                    {
                        throw new Exception("IServiceProvider is not available");
                    }

                    var arguments = new object[parameters.Length];
                    arguments[0] = context;
                    for(var index = 1; index != parameters.Length; ++index)
                    {
                        var serviceType = parameters[index].ParameterType;
                        arguments[index] = serviceProvider.GetService(serviceType);
                        if (arguments[index] == null)
                        {
                            throw new Exception(string.Format("No service for type '{0}' has been registered.", serviceType));
                        }

                    }
                    return (Task)methodinfo.Invoke(instance, arguments);
                };
            });
        }
    }
}