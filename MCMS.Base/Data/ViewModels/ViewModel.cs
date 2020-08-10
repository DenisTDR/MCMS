using MCMS.Base.Display.ModelDisplay.Attributes;

namespace MCMS.Base.Data.ViewModels
{
    public class ViewModel : IViewModel
    {
        [TableColumn(Hidden = true)]
        [DetailsField(Hidden = true)]
        public virtual string Id { get; set; }
    }
}