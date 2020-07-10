using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MCMS.Base.Builder
{
    public class LayoutIncludesOptions
    {
        private readonly List<string> _includesForPages = new List<string>();
        public ReadOnlyCollection<string> GetAllForPage => _includesForPages.AsReadOnly();
        private readonly List<string> _includesForModals = new List<string>();
        public ReadOnlyCollection<string> GetAllForModals => _includesForModals.AsReadOnly();

        public void AddForPages(string layoutIncludePath)
        {
            _includesForPages.Add(layoutIncludePath);
        }

        public void AddForModals(string layoutIncludePath)
        {
            _includesForModals.Add(layoutIncludePath);
        }
    }
}