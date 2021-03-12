using MCMS.Base.Helpers;

namespace MCMS.Display
{
    public abstract class WithUniqueId
    {
        private string _uniqueId;
        public string UniqueId => _uniqueId ??= BuildUniqueId();
        protected abstract string GetHashSource();

        private string BuildUniqueId()
        {
            return FastHash.Hash(GetHashSource() ?? Utils.GenerateRandomHexString());
        }

        public void SetCustomId(string idToSet)
        {
            _uniqueId = idToSet;
        }
    }
}