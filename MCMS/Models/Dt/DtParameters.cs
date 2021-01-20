using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MCMS.Models.Dt
{
    public class DtParameters
    {
        [Required] public int Start { get; set; }
        [Required] public int Length { get; set; }
        [Required] public int Draw { get; set; }
        [Required] public List<DtColumn> Columns { get; set; }
        [Required] public List<DtOrder> Order { get; set; }
        [Required] public DtSearch Search { get; set; }
    }
}