using System.Collections.Generic;

namespace MCMS.Logging.AuditLogs
{
    public interface IMAuditLogger<out TCategoryName>
    {
        void Log(Dictionary<string, object> data = null);
        void UpdateLog(Dictionary<string, object> data = null);
    }
}