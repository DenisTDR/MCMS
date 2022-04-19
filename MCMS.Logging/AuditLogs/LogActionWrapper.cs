using System;

namespace MCMS.Logging.AuditLogs
{
    public class LogActionWrapper<T>
    {
        public T Log { get; set; }
        public ActionType Type { get; set; }

        public LogActionWrapper(T log, ActionType type = ActionType.Add)
        {
            if (log == null)
            {
                throw new Exception("Null log object");
            }
            Log = log;
            Type = type;
        }
    }

    public enum ActionType
    {
        Add,
        Update
    }
}