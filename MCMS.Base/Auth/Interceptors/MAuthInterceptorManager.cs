using System.Collections.Generic;
using System.Threading.Tasks;
using MCMS.Base.Attributes;
using Microsoft.AspNetCore.Identity;

namespace MCMS.Base.Auth.Interceptors
{
    [Service]
    public class MAuthInterceptorManager : IMAuthInterceptor
    {
        private readonly IEnumerable<IMAuthInterceptor> _interceptors;

        public MAuthInterceptorManager(IEnumerable<IMAuthInterceptor> interceptors)
        {
            _interceptors = interceptors;
        }

        public async Task<AuthInterceptorResult> OnBeforeSignIn(string username, SignInType type)
        {
            foreach (var interceptor in _interceptors)
            {
                var crtResult = await interceptor.OnBeforeSignIn(username, type);
                if (crtResult is { Succeeded: false })
                {
                    return crtResult;
                }
            }

            return new AuthInterceptorResult(true);
        }

        public async Task<SignInResult> OnAfterSignIn(User user, SignInResult signInResult, SignInType type)
        {
            foreach (var interceptor in _interceptors)
            {
                var crtResult = await interceptor.OnAfterSignIn(user, signInResult, type);
                if (crtResult is { Succeeded: false })
                {
                    return crtResult;
                }
            }

            return signInResult;
        }

        public async Task<AuthInterceptorResult> OnBeforeForgotPassword(string username, SignInType type)
        {
            foreach (var interceptor in _interceptors)
            {
                var crtResult = await interceptor.OnBeforeForgotPassword(username, type);
                if (crtResult is { Succeeded: false })
                {
                    return crtResult;
                }
            }

            return new AuthInterceptorResult(true);
        }
    }
}