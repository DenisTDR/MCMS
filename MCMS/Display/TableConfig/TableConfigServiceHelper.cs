using System;
using System.Linq;
using MCMS.Base.Extensions;
using MCMS.Controllers.Ui;

namespace MCMS.Display.TableConfig
{
    public class TableConfigServiceHelper
    {
        public static Type GetTypeForUiController(Type uiControllerType)
        {
            if (uiControllerType == null)
            {
                throw new ArgumentException(nameof(uiControllerType));
            }

            if (!uiControllerType.IsSubclassOfGenericType(typeof(GenericAdminUiController<,,,>)))
            {
                throw new ArgumentNullException(nameof(uiControllerType));
            }

            var args = uiControllerType
                .GetGenericArgumentsOfGenericTypeImplementation(typeof(GenericAdminUiController<,,,>)).ToList();
            args.Insert(3, uiControllerType);
            return typeof(TableConfigForControllerService<,,,,>).MakeGenericType(args.ToArray());
        }
    }
}