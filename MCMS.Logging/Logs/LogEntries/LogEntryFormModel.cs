using System.ComponentModel.DataAnnotations;
using MCMS.Base.Data.FormModels;

namespace MCMS.Logging.Logs.LogEntries
{
    public class LogEntryFormModel : IFormModel
    {
        public string Type { get; set; }

        public string Title { get; set; }

        [DataType(DataType.MultilineText)] public string Data { get; set; }

        [DataType(DataType.MultilineText)] public string RawData { get; set; }

        public string Context { get; set; }
    }
}