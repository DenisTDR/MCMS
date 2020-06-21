using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MCMS.Controllers
{
    public class UiMockController: UiController
    {
        public override Task<IActionResult> Index()
        {
            throw new System.NotImplementedException();
        }
    }
}