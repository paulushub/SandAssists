using System;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components
{
    public sealed class BuildComponents : BuildComponentEx
    {
        #region Private Fields

        private List<BuildComponent> _listItems;

        #endregion

        #region Constructors and Destructor

        public BuildComponents(BuildAssembler assembler,
            XPathNavigator configuration)
            : base(assembler, configuration)
        {
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return (_listItems == null || _listItems.Count == 0);
            }
        }

        public int Count
        {
            get
            {
                if (_listItems != null)
                {
                    return _listItems.Count;
                }

                return 0;
            }
        }

        public BuildComponent this[int index]
        {
            get
            {
                if (_listItems != null)
                {
                    return _listItems[index];
                }

                return null;
            }
        }

        public IList<BuildComponent> Items
        {
            get
            {
                return _listItems;
            }
        }

        #endregion

        #region Public Methods

        public void Add(BuildComponent[] components)
        {
            if (components == null || components.Length == 0)
            {
                return;
            }

            if (_listItems == null)
            {
                _listItems = new List<BuildComponent>(components);
            }
            else
            {
                _listItems.AddRange(components);
            }
        }

        public void Add(IList<BuildComponent> components)
        {
            if (components == null || components.Count == 0)
            {
                return;
            }

            if (_listItems == null)
            {
                _listItems = new List<BuildComponent>(components);
            }
            else
            {
                _listItems.AddRange(components);
            }
        }

        public override void Apply(XmlDocument document, string key)
        {
            if (_listItems == null || _listItems.Count == 0)
            {
                return;
            }

            int itemCount = _listItems.Count;
            for (int i = 0; i < itemCount; i++)
            {
                BuildComponent component = _listItems[i];
                if (component != null)
                {
                    component.Apply(document, key);
                }
            }
        }

        #endregion
    }
}
