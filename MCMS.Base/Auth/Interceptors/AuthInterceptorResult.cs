namespace MCMS.Base.Auth.Interceptors
{
    public class AuthInterceptorResult
    {
        public AuthInterceptorResult(string reason)
        {
            Reason = reason;
        }

        public AuthInterceptorResult(bool succeeded)
        {
            Succeeded = succeeded;
        }


        public bool Succeeded { get; set; }
        public string Reason { get; set; }
    }
}