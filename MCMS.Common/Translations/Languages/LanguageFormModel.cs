using MCMS.Base.Data.FormModels;

namespace MCMS.Common.Translations.Languages
{
    public class LanguageFormModel: IFormModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}