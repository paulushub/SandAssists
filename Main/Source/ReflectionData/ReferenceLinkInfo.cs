// Copyright ｩ Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

namespace Sandcastle.ReflectionData
{
    public sealed class ReferenceLinkInfo
    {
        #region Private Fields

        // stored data     
        private bool           _preferOverload;
        private string         _target;
        private string         _displayTarget;
        private ReferenceLinkDisplayOptions _options;
        private XPathNavigator _contents;

        #endregion

        #region Constructors and Destructor

        private ReferenceLinkInfo()
        {
            _options = ReferenceLinkDisplayOptions.Default;
        }

        #endregion

        #region Public Properties

        // data accessors

        public string Target
        {
            get
            {
                return _target;
            }
        }

        public string DisplayTarget
        {
            get
            {
                return _displayTarget;
            }
        }

        public ReferenceLinkDisplayOptions DisplayOptions
        {
            get
            {
                return _options;
            }
        }

        public bool PreferOverload
        {
            get
            {
                return _preferOverload;
            }
        }

        public XPathNavigator Contents
        {
            get
            {
                return _contents;
            }
        }

        #endregion

        #region Public Methods

        // creation logic
        public static ReferenceLinkInfo Create(XPathNavigator element)
        {
            if (element == null)
                throw new ArgumentNullException("element");

            ReferenceLinkInfo info = new ReferenceLinkInfo();

            info._target = element.GetAttribute("target", String.Empty);
            if (String.IsNullOrEmpty(info._target)) return (null);

            info._displayTarget = element.GetAttribute("display-target", String.Empty);

            string showContainer = element.GetAttribute("show-container", String.Empty);
            if (String.IsNullOrEmpty(showContainer))
                showContainer = element.GetAttribute("qualified", String.Empty);

            if (!String.IsNullOrEmpty(showContainer))
            {
                if (String.Compare(showContainer, Boolean.TrueString, true) == 0)
                {
                    info._options = info._options | ReferenceLinkDisplayOptions.ShowContainer;
                }
                else if (String.Compare(showContainer, Boolean.FalseString, true) == 0)
                {
                    info._options = info._options & ~ReferenceLinkDisplayOptions.ShowContainer;
                }
                else
                {
                    return (null);
                }
            }

            string showTemplates = element.GetAttribute("show-templates", String.Empty);
            if (!String.IsNullOrEmpty(showTemplates))
            {
                if (String.Compare(showTemplates, Boolean.TrueString, true) == 0)
                {
                    info._options = info._options | ReferenceLinkDisplayOptions.ShowTemplates;
                }
                else if (String.Compare(showTemplates, Boolean.FalseString, true) == 0)
                {
                    info._options = info._options & ~ReferenceLinkDisplayOptions.ShowTemplates;
                }
                else
                {
                    return (null);
                }
            }

            string showParameters = element.GetAttribute("show-parameters", String.Empty);
            if (!String.IsNullOrEmpty(showParameters))
            {
                if (String.Compare(showParameters, Boolean.TrueString, true) == 0)
                {
                    info._options = info._options | ReferenceLinkDisplayOptions.ShowParameters;
                }
                else if (String.Compare(showParameters, Boolean.FalseString, true) == 0)
                {
                    info._options = info._options & ~ReferenceLinkDisplayOptions.ShowParameters;
                }
                else
                {
                    return (null);
                }
            }  

            string preferOverload = element.GetAttribute("prefer-overload", String.Empty);
            if (String.IsNullOrEmpty(preferOverload))
                preferOverload = element.GetAttribute("auto-upgrade", String.Empty);
            if (!String.IsNullOrEmpty(preferOverload))
            {
                if (String.Compare(preferOverload, Boolean.TrueString, true) == 0)
                {
                    info._preferOverload = true;
                }
                else if (String.Compare(preferOverload, Boolean.FalseString, true) == 0)
                {
                    info._preferOverload = false;
                }
                else
                {
                    return (null);
                }
            }

            info._contents = element.Clone();
            if (!info._contents.MoveToFirstChild())
                info._contents = null;

            return (info);
        }

        #endregion
    }
}
