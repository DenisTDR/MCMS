using System.Collections.Generic;

namespace MCMS.Base.Files.UploadPurpose
{
    public class UploadPurposeOptions
    {
        private Dictionary<string, IFileUploadPurpose> _registeredPurposes =
            new();

        public void Register(string purposeName, IFileUploadPurpose purpose)
        {
            if (!_registeredPurposes.ContainsKey(purposeName))
            {
                _registeredPurposes.Add(purposeName, purpose);
            }
        }

        public IFileUploadPurpose Get(string purposeName)
        {
            if (_registeredPurposes.ContainsKey(purposeName))
            {
                return _registeredPurposes[purposeName];
            }

            return null;
        }

        public bool TryGetValue(string purposeName, out IFileUploadPurpose purpose)
        {
            if (_registeredPurposes.ContainsKey(purposeName))
            {
                purpose = _registeredPurposes[purposeName];
                return true;
            }

            purpose = null;
            return false;
        }
    }
}