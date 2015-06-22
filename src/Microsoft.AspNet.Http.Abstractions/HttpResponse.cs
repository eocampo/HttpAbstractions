// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.AspNet.Http
{
    public abstract class HttpResponse
    {
        public abstract HttpContext HttpContext { get; }

        public abstract int StatusCode { get; set; }

        public abstract IHeaderDictionary Headers { get; }

        public abstract Stream Body { get; set; }

        public abstract long? ContentLength { get; set; }

        public abstract string ContentType { get; set; }

        public abstract IResponseCookies Cookies { get; }

        public abstract bool HasStarted { get; }

        public abstract void OnResponseStarting(Func<object, Task> callback, object state);

        public virtual void OnResponseStarting(Func<Task> callback) => OnResponseStarting(s => callback(), state: null);

        public virtual void OnResponseStarting(Action callback) => OnResponseStarting(s => callback(), state: null);

        public virtual void OnResponseStarting(Action<object> callback, object state) =>
            OnResponseStarting(s =>
            {
                callback(s);
                return Task.FromResult(0);
            }, state);

        public abstract void OnResponseCompleted(Func<object, Task> callback, object state);

        public virtual void OnResponseCompleted(Func<Task> callback) => OnResponseCompleted(s => callback(), state: null);

        public virtual void OnResponseCompleted(Action callback) => OnResponseCompleted(s => callback(), state: null);

        public virtual void OnResponseCompleted(Action<object> callback, object state) =>
            OnResponseCompleted(s =>
            {
                callback(s);
                return Task.FromResult(0);
            }, state);

        public virtual void Redirect(string location) => Redirect(location, permanent: false);

        public abstract void Redirect(string location, bool permanent);
    }
}
