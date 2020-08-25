using System.Threading.Tasks;
using MCMS.Display.ModelDisplay;

namespace MCMS.Controllers.Ui
{
    public interface IGenericAdminUiController
    {
        public Task<ModelDisplayTableConfig> TableConfigForIndex();
    }
}