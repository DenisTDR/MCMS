using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MCMS.Base.Attributes.JsonConverters;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay.Attributes;
using Newtonsoft.Json;

namespace MCMS.Logging.AuditLogEntries
{
    [Display(Name = "LogEntry")]
    public class AuditLogEntryViewModel : ViewModel
    {
        [JsonConverter(typeof(ToStringJsonConverter))]
        [TableColumn(DbColumn = "User.FirstName",
            DbFuncFormat = "MDbFunctions.Concat({0}, x.User.LastName, x.User.Email)")]
        public MCMS.Base.Auth.User Author { get; set; }

        [TableColumn] public string Category { get; set; }

        [TableColumn] public string Ip { get; set; }
        [TableColumn] public string Controller { get; set; }
        [TableColumn] public string Action { get; set; }
        [TableColumn] public string Path { get; set; }
        [TableColumn] public string TraceIdentifier { get; set; }
        [TableColumn] public DateTime Created { get; set; }
        [TableColumn] public string SerializedData { get; set; }

        public override string ToString()
        {
            return Id;
        }
    }
}