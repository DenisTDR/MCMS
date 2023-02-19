using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MCMS.Base.Auth.Interceptors
{
    public abstract class MAuthInterceptor: IMAuthInterceptor
    {
        public virtual Task<AuthInterceptorResult> OnBeforeSignIn(string username, SignInType type)
        {
            return Task.FromResult(null as AuthInterceptorResult);
        }

        public virtual Task<SignInResult> OnAfterSignIn(User user, SignInResult signInResult, SignInType type)
        {
            return Task.FromResult(null as SignInResult);
        }

        public virtual Task<AuthInterceptorResult> OnBeforeForgotPassword(string username, SignInType type)
        {
            return Task.FromResult(null as AuthInterceptorResult);
        }
    }
}