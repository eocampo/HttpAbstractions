// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Http.Features.Authentication;

namespace Microsoft.AspNet.Http.Authentication
{
    public abstract class AuthenticationManager
    {
        public abstract IEnumerable<AuthenticationDescription> GetAuthenticationSchemes();

        public abstract void Authenticate(AuthenticateContext context);

        public abstract Task AuthenticateAsync(AuthenticateContext context);

        // REVIEW: Move to extension methods?
        public ClaimsPrincipal Authenticate(string authenticationScheme)
        {
            var context = new AuthenticateContext(authenticationScheme);
            Authenticate(context);
            return context.Principal;
        }

        public async Task<ClaimsPrincipal> AuthenticateAsync(string authenticationScheme)
        {
            var context = new AuthenticateContext(authenticationScheme);
            await AuthenticateAsync(context);
            return context.Principal;
        }

        public virtual void Challenge()
        {
            Challenge(authenticationScheme: null, properties: null);
        }

        public virtual void Challenge(AuthenticationProperties properties)
        {
            Challenge(authenticationScheme: null, properties: properties);
        }

        public virtual void Challenge(string authenticationScheme)
        {
            Challenge(authenticationScheme: authenticationScheme, properties: null);
        }

        public abstract void Challenge(string authenticationScheme, AuthenticationProperties properties);

        public void SignIn(string authenticationScheme, ClaimsPrincipal principal)
        {
            SignIn(authenticationScheme, principal, properties: null);
        }

        public abstract void SignIn(string authenticationScheme, ClaimsPrincipal principal, AuthenticationProperties properties);

        public virtual void SignOut()
        {
            SignOut(authenticationScheme: null, properties: null);
        }

        public abstract void SignOut(string authenticationScheme);

        public abstract void SignOut(string authenticationScheme, AuthenticationProperties properties);
    }
}
