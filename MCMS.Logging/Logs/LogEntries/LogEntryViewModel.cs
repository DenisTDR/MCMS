using System;
using System.ComponentModel.DataAnnotations;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay.Attributes;
using Newtonsoft.Json;

namespace MCMS.Logging.Logs.LogEntries
{
    [Display(Name = "LogEntry")]
    public class LogEntryViewModel : ViewModel
    {
        [TableColumn] public string Type { get; set; }

        [TableColumn] public string Title { get; set; }

        [TableColumn] public string Data { get; set; }

        [TableColumn] public string RawData { get; set; }

        [TableColumn] public string Context { get; set; }

        [JsonIgnore]
        [TableColumn(Hidden = true)]
        [DetailsField(Hidden = true)]
        public DateTime Created { get; set; }

        [TableColumn(DbColumn = "Created")] public string Timestamp => Created.ToString("u");

        public override string ToString()
        {
            return Id;
        }
    }
}