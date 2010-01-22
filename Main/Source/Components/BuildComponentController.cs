using System;
using System.Collections.Generic;

using Sandcastle.Components.Others;

namespace Sandcastle.Components
{
    public class BuildComponentController : MarshalByRefObject
    {
        private static BuildComponentController _buildController;

        private IList<VersionInfo> _currentVersions;
        private Dictionary<string, VersionInfo> _versionInfo;

        public BuildComponentController()
        {
            _versionInfo     = new Dictionary<string, VersionInfo>(
                StringComparer.CurrentCultureIgnoreCase);
            _currentVersions = new List<VersionInfo>();
        }

        public bool HasVersions
        {
            get
            {
                return (_versionInfo != null && _versionInfo.Count != 0);
            }
        }

        public IList<VersionInfo> CurrentVersions
        {
            get
            {
                return _currentVersions;
            }
        }

        public IDictionary<string, VersionInfo> Versions
        {
            get
            {
                return _versionInfo;
            }
        }

        public static BuildComponentController Controller
        {
            get
            {
                if (_buildController == null)
                {
                    _buildController = new BuildComponentController();
                }

                return _buildController;
            }
        }

        public void AddVersion(VersionInfo versionInfo)
        {
            if (versionInfo == null || 
                String.IsNullOrEmpty(versionInfo.AssemblyName))
            {
                return;
            }

            _versionInfo[versionInfo.AssemblyName] = versionInfo;
        }  

        public void ClearVersions()
        {
            _currentVersions.Clear();
        }

        public void UpdateVersion(string itemName)
        {
            if (String.IsNullOrEmpty(itemName))
            {
                return;
            }

            VersionInfo curVersion = null;
            if (_versionInfo.TryGetValue(itemName, out curVersion))
            {
                _currentVersions.Add(curVersion);
            }
        }
    }
}