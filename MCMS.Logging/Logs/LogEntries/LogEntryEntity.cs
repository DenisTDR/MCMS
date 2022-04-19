using System.ComponentModel.DataAnnotations.Schema;
using MCMS.Base.Data.Entities;

namespace MCMS.Logging.Logs.LogEntries
{
    [Table("LogEntries")]
    public class LogEntryEntity : Entity
    {
        public string Type { get; set; }

        public string Title { get; set; }

        public string Data { get; set; }

        public string RawData { get; set; }

        public string Context { get; set; }

        public override string ToString()
        {
            return Id;
        }
    }
}