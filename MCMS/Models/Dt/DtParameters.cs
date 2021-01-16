using System.Collections.Generic;

namespace MCMS.Models.Dt
{
    public class DtParameters
    {
        public int Start { get; set; }
        public int Length { get; set; }
        public int Draw { get; set; }
        public List<DtColumn> Columns { get; set; }
        public List<DtOrder> Order { get; set; }
        public DtSearch Search { get; set; }
    }
}