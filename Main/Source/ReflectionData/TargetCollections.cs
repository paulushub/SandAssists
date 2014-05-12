using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

using Sandcastle.ReflectionData.References;
using Sandcastle.ReflectionData.Targets;

namespace Sandcastle.ReflectionData
{
    public sealed class TargetCollections : TargetCollection
    {
        #region Private Fields

        private ReferenceLinkType _localLink;
        private ReferenceLinkType _msdnLink;

        private TargetStorage _localStorage;
        private TargetStorage _msdnStorage;

        #endregion

        #region Constructors and Destructor

        public TargetCollections()
        {
            _localLink = ReferenceLinkType.None;
            _msdnLink  = ReferenceLinkType.None;
        }

        public TargetCollections(TargetStorage local, TargetStorage msdn,
            ReferenceLinkType localLink, ReferenceLinkType msdnLink)
        {
            _localLink    = localLink;
            _msdnLink     = msdnLink;

            _localStorage = local;
            _msdnStorage  = msdn;
        }

        #endregion

        #region Public Properties

        public override int Count
        {
            get
            {
                int targetCount = 0;
                if (_localStorage != null)
                {
                    targetCount += _localStorage.Count;
                }
                if (_msdnStorage != null)
                {
                    targetCount += _msdnStorage.Count;
                }

                return targetCount;
            }
        }

        public override Target this[string id]
        {
            get
            {
                Target target             = null;
                this.RecentLinkType       = ReferenceLinkType.None;
                this.RecentLinkTypeIsMsdn = false;

                if (_localStorage != null && _localStorage.Count != 0)
                {
                    target = _localStorage[id];
                    if (target != null)
                    {
                        this.RecentLinkType       = _localLink;
                        this.RecentLinkTypeIsMsdn = false;

                        return target;
                    }
                }
                if (_msdnStorage != null && _msdnStorage.Count != 0)
                {
                    target = _msdnStorage[id];
                    if (target != null)
                    {
                        this.RecentLinkType       = _msdnLink;
                        this.RecentLinkTypeIsMsdn = true;

                        return target;
                    }
                }

                return target;
            }
        }

        public ReferenceLinkType LocalLink
        {
            get
            {
                return _localLink;
            }
        }

        public ReferenceLinkType MsdnLink
        {
            get
            {
                return _msdnLink;
            }
        }

        public TargetStorage Local
        {
            get
            {
                return _localStorage;
            }
        }

        public TargetStorage Msdn
        {
            get
            {
                return _msdnStorage;
            }
        }

        #endregion

        #region Public Methods

        public override bool Contains(string id)
        {
            if (_localStorage != null && _localStorage.Contains(id))
            {
                return true;
            }
            if (_msdnStorage != null && _msdnStorage.Contains(id))
            {
                return true;
            }
            return false;
        }

        #endregion
    }
}
