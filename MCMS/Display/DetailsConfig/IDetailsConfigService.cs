using System;
using System.Collections.Generic;
using MCMS.Base.Data.ViewModels;
using MCMS.Base.Display.ModelDisplay;

namespace MCMS.Display.DetailsConfig
{
    public interface IDetailsConfigService
    {
        List<DetailsField> GetDetailsFields(Type viewModelType = null);
    }

    public interface IDetailsConfigServiceT<T> : IDetailsConfigService where T : class, IViewModel
    {
        DetailsViewModelT<T> Wrap(T model);
        Type ViewModelType { get; }
    }
}