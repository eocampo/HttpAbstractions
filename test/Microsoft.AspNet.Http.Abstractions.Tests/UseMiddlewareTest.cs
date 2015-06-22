// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Builder.Internal;
using Xunit;

namespace Microsoft.AspNet.Http
{
    public class UseMiddlewareTest
    {
        [Fact]
        public void UseMiddleware_WithNoParameters_ThrowsException()
        {
            var mockServiceProvider = new DummyServiceProvider();
            var builder = new ApplicationBuilder(mockServiceProvider);
            builder.UseMiddleware(typeof(MiddlewareNoParametersStub));
            var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());
            Assert.Equal("Middleware Invoke method must take first argument of HttpContext", exception.Message); 
        }

        [Fact]
        public void UseMiddleware_NonTaskReturnType_ThrowsException()
        {
            var mockServiceProvider = new DummyServiceProvider();
            var builder = new ApplicationBuilder(mockServiceProvider);
            builder.UseMiddleware(typeof(MiddlewareNonTaskReturnStub));
            var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());
            Assert.Equal("Invoke does not return an object of type Task", exception.Message);
        }

        [Fact]
        public void UseMiddleware_NoInvokeMethod_ThrowsException()
        {          
            var mockServiceProvider = new DummyServiceProvider();
            var builder = new ApplicationBuilder(mockServiceProvider);
            builder.UseMiddleware(typeof(MiddlewareNoInvokeStub));
            var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());
            Assert.Equal("No public Invoke method found", exception.Message);
        }

        private class DummyServiceProvider : IServiceProvider
        {
            public object GetService(Type serviceType)
            {
                return null;
            }
        }

        private class MiddlewareNoParametersStub
        {
            public MiddlewareNoParametersStub(RequestDelegate next)
            {
            }

            public Task Invoke()
            {
                return Task.FromResult(0);
            }
        }

        private class MiddlewareNonTaskReturnStub
        {
            public MiddlewareNonTaskReturnStub(RequestDelegate next)
            {
            }

            public int Invoke()
            {
                return 0;
            }
        }
         
        private class MiddlewareNoInvokeStub
        {
            public MiddlewareNoInvokeStub(RequestDelegate next)
            {
            }
        }
    }
}