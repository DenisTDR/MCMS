using System.Collections.Generic;
using System.Threading.Tasks;

namespace MCMS.Base.Repositories
{
    public interface ITranslationsRepository
    {
        Task<string> GetValue(string slug, string lang = null);
        Task<string> GetValueOrSlug(string slug, string lang = null);
        Task<string> Format(string slug, params object[] args);
        Task<string> Format(string slug, string lang = null, params object[] args);
        string Language { get; }
        Task<Dictionary<string, string>> GetAll(string langCode = null, string tag = null);
    }
}