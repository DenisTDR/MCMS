using MCMS.Base.SwaggerFormly.Formly;

namespace MCMS.Base.Data.ViewModels
{
    public class ViewModel : IViewModel
    {
        [FormlyIgnore] public string Id { get; set; }
    }
}