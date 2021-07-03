using System.Collections.Generic;

namespace MCMS.Logging
{
    public interface IMAuditLogger<out TCategoryName>
    {
        void Log(Dictionary<string, object> data = null);
        void UpdateLog(Dictionary<string, object> data = null);
    }
}