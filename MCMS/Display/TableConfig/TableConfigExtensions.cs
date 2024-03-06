using System.Collections.Generic;
using MCMS.Display.Link;

namespace MCMS.Display.TableConfig
{
    public static class TableConfigExtensions
    {
        public static IEnumerable<MRichLink> GetAllItemActions(this TableConfig config)
        {
            if (config.ItemActions is { Count: > 0 })
            {
                foreach (var configItemAction in config.ItemActions)
                {
                    yield return configItemAction;
                }
            }

            if (config.DefaultItemAction != null)
            {
                yield return config.DefaultItemAction;
            }

            if (config.BatchActions is { Count: > 0 })
            {
                foreach (var configBatchAction in config.BatchActions)
                {
                    yield return configBatchAction;
                }
            }
        }
    }
}