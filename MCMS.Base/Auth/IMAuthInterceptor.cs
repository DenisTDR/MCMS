using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MCMS.Base.Auth
{
    public interface IMAuthInterceptor
    {
        Task<SignInResult> OnSignIn(User user, SignInResult signInResult, SignInType type);
    }

    public enum SignInType
    {
        Unknown,
        Dashboard,
        Api
    }
}