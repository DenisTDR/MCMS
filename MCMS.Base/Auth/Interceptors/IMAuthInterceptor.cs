using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MCMS.Base.Auth.Interceptors
{
    public interface IMAuthInterceptor
    {
        Task<AuthInterceptorResult> OnBeforeSignIn(string username, SignInType type);
        Task<SignInResult> OnAfterSignIn(User user, SignInResult signInResult, SignInType type);
        Task<AuthInterceptorResult> OnBeforeForgotPassword(string username, SignInType type);
    }

    public enum SignInType
    {
        Unknown,
        Dashboard,
        Api
    }
}