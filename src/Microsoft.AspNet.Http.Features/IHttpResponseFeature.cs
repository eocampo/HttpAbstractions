// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.AspNet.Http.Features
{
    public interface IHttpResponseFeature
    {
        int StatusCode { get; set; }
        string ReasonPhrase { get; set; }
        IDictionary<string, string[]> Headers { get; set; }
        Stream Body { get; set; }
        bool HasStarted { get; }
        void OnResponseStarting(Action<object> callback, object state);
        void OnResponseCompleted(Action<object> callback, object state);
    }
}
