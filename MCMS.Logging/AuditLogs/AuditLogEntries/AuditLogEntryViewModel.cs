using System;
using System.ComponentModel.DataAnnotations;
using MCMS.Base.Attributes.JsonConverters;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay;
using MCMS.Base.Display.ModelDisplay.Attributes;
using Newtonsoft.Json;

namespace MCMS.Logging.AuditLogs.AuditLogEntries
{
    [Display(Name = "LogEntry")]
    public class AuditLogEntryViewModel : ViewModel
    {
        [JsonConverter(typeof(ToStringJsonConverter))]
        [TableColumn(DbColumn = "Author.FirstName",
            DbFuncFormat = "MDbFunctions.Concat({0}, x.Author.LastName, x.Author.Email)")]
        public MCMS.Base.Auth.User Author { get; set; }

        [TableColumn] public string Category { get; set; }

        [TableColumn] public string Ip { get; set; }
        [TableColumn] public string Controller { get; set; }
        [TableColumn] public string Action { get; set; }
        [TableColumn] public string Path { get; set; }
        [TableColumn] public string TraceIdentifier { get; set; }

        [TableColumn] public DateTime Begin { get; set; }

        [TableColumn] public DateTime End { get; set; }

        [TableColumn(Searchable = ServerClient.None)]
        public int Duration => (int)(End - Begin).TotalMilliseconds;

        [TableColumn] public string SerializedData { get; set; }

        public override string ToString()
        {
            return Id;
        }
    }
}