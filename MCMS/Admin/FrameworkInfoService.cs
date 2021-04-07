using Microsoft.Extensions.Options;

namespace MCMS.Admin
{
    public class FrameworkInfoService
    {
        private readonly FrameworkLibsDetails _details;

        public FrameworkInfoService(IOptions<FrameworkLibsDetails> detailsOptions)
        {
            _details = detailsOptions.Value;
        }

        public FrameworkLibsDetails GetDetails()
        {
            return _details;
        }
    }
}