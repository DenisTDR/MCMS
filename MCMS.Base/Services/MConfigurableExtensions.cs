using System;

namespace MCMS.Base.Services
{
    public static class MConfigurableExtensions
    {
        public static T Configure<T>(this T service, Action<T> configure) where T : IMConfigurable
        {
            configure(service);
            return service;
        }
    }
}