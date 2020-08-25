using System.Collections.Generic;
using MCMS.Display.Link;

namespace MCMS.Display.ModelDisplay
{
    public interface IModelDisplayConfigForControllerService : IModelDisplayConfigService
    {
        public List<MRichLink> GetTableItemActions(dynamic viewBag, bool excludeDefault = false);
    }
}