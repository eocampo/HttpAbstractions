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
            var mockServiceProvider = new MockServiceProvider();
            var builder = new ApplicationBuilder(mockServiceProvider);
            builder.UseMiddleware(typeof(MiddlewareNoParameters));
            var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());
            Assert.Equal("Middleware Invoke method must take first argument of HttpContext", exception.Message); 
        }

        [Fact]
        public void UseMiddleware_NonTaskReturnType_ThrowsException()
        {
            var mockServiceProvider = new MockServiceProvider();
            var builder = new ApplicationBuilder(mockServiceProvider);
            builder.UseMiddleware(typeof(MiddlewareNonTaskReturn));
            var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());
            Assert.Equal("Invoke does not return an object of type Task", exception.Message);
        }

        [Fact]
        public void UseMiddleware_NoInvokeMethod_ThrowsException()
        {          
            var mockServiceProvider = new MockServiceProvider();
            var builder = new ApplicationBuilder(mockServiceProvider);
            builder.UseMiddleware(typeof(MiddlewareNoInvoke));
            var exception = Assert.Throws<InvalidOperationException>(() => builder.Build());
            Assert.Equal("No public Invoke method found", exception.Message);
        }

        private class MockServiceProvider : IServiceProvider
        {
            public object GetService(Type serviceType)
            {
                return null;
            }
        }

        private class MiddlewareNoParameters
        {
            public MiddlewareNoParameters(RequestDelegate next)
            {
            }

            public Task Invoke()
            {
                return Task.FromResult(0);
            }
        }

        private class MiddlewareNonTaskReturn
        {
            public MiddlewareNonTaskReturn(RequestDelegate next)
            {
            }

            public int Invoke()
            {
                return 0;
            }
        }
         
        private class MiddlewareNoInvoke
        {
            public MiddlewareNoInvoke(RequestDelegate next)
            {
            }
        }
    }
}