using System;
using System.ComponentModel.DataAnnotations;

namespace MCMS.Models.Dt
{
    public class DtOrder
    {
        [Range(0, int.MaxValue)] public int Column { get; set; }
        public DtOrderDir Dir { get; set; }
    }

    public enum DtOrderDir
    {
        Asc,
        Desc
    }
}