using System.Threading.Tasks;

namespace MCMS.Base.Repositories
{
    public interface ITranslationsRepository
    {
         Task<string> GetValue(string slug, string lang = null);
         Task<string> GetValueOrSlug(string slug, string lang = null);
         
         Task<string> Format(string slug, params object[] args);
         Task<string> Format(string slug, string lang = null, params object[] args);
    }
}