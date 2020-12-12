using System.Collections.Generic;
using MCMS.Display.Link;

namespace MCMS.Display.ModelDisplay
{
    public interface IModelDisplayConfigForControllerService : IModelDisplayConfigService
    {
        public List<MRichLink> GetItemActions(dynamic viewBag, bool excludeDefault = false);
    }
}