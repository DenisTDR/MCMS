using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MCMS.Data;
using Microsoft.EntityFrameworkCore;

namespace MCMS.Common.Translations.Languages
{
    public class LanguagesRepository : Repository<LanguageEntity>
    {
        public LanguagesRepository(BaseDbContext dbContext) : base(dbContext)
        {
        }

        public Task<List<string>> GetLanguagesCodes()
        {
            return Queryable.Select(l => l.Code).OrderBy(c => c).ToListAsync();
        }
    }
}