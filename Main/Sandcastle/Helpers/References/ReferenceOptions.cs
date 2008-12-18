using System;
using System.Xml;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

using Sandcastle.Contents;

namespace Sandcastle.References
{
    /// <summary>
    /// This provides the customization options for the reference (or API) 
    /// documentations.
    /// </summary>
    /// <remarks>
    /// The options are accessible from a <see cref="ReferenceGroup.Options"/>, and
    /// are separated into a separate class to make the extensions and customizations
    /// flexible.
    /// </remarks>
    /// <seealso cref="ReferenceGroup.Options"/>
    /// <seealso cref="OptionItem"/>
    /// <seealso cref="OptionContent"/>
    [Serializable]
    public sealed class ReferenceOptions : BuildObject<ReferenceOptions>
    {
        #region Public Const Fields

        public const string AutoDoc  = "AutoDocumentation";
        public const string Missing  = "MissingDocumentation";
        public const string Included = "IncludedDocumentation";

        #endregion

        #region Private Fields

        private OptionContent  _autoDocument;
        private OptionContent  _missingDocument;
        private OptionContent  _includedDocument;

        private ReferenceNamer _refNamer;
        private ReferenceNamingMethod _refNaming;

        #endregion

        #region Constructor and Destructor

        public ReferenceOptions()
        {
            _refNamer         = ReferenceNamer.Orcas;
            _refNaming        = ReferenceNamingMethod.Guid;
            _autoDocument     = new OptionContent(AutoDoc);
            _missingDocument  = new OptionContent(Missing);
            _includedDocument = new OptionContent(Included);

            CreateContents();
        }

        public ReferenceOptions(ReferenceOptions source)
            : base(source)
        {
            _refNaming        = source._refNaming;
            _refNamer         = source._refNamer;
            _autoDocument     = source._autoDocument;
            _missingDocument  = source._missingDocument;
            _includedDocument = source._includedDocument;
        }

        #endregion

        #region Public Properties

        public bool? this[string category, string option]
        {
            get
            {
                OptionItem item = GetItem(category, option);
                if (item != null)
                {
                    return item.Value;
                }

                return null; 
            }
            set
            {
                OptionItem item = GetItem(category, option);
                if (item == null)
                {
                    return;
                }
                if (value == null)
                {
                    item.Reset();
                }
                else
                {
                    item.Value = value.Value;
                }
            }
        }

        public OptionContent AutoDocument
        {
            get 
            { 
                return _autoDocument; 
            }
            set 
            { 
                _autoDocument = value; 
            }
        }

        public OptionContent MissingDocument
        {
            get 
            { 
                return _missingDocument; 
            }
            set 
            { 
                _missingDocument = value; 
            }
        }

        public OptionContent IncludedDocument
        {
            get 
            { 
                return _includedDocument; 
            }
            set 
            { 
                _includedDocument = value; 
            }
        }

        public ReferenceNamer Namer
        {
            get
            {
                return _refNamer;
            }
            set
            {
                _refNamer = value;
            }
        }

        public ReferenceNamingMethod Naming
        {
            get
            {
                return _refNaming;
            }
            set
            {
                _refNaming = value;
            }
        }

        #endregion

        #region Public Methods

        public OptionItem GetItem(string category, string option)
        {
            OptionItem item = null;
            if (String.IsNullOrEmpty(category) || String.IsNullOrEmpty(option))
            {
                return item;
            }

            // 1. For the auto-documentation options...
            if (String.Equals(category, AutoDoc,
                StringComparison.CurrentCultureIgnoreCase))
            {
                if (_autoDocument != null)
                {
                    item = _autoDocument[option];
                }

                return item;
            }
            // 2. For the missing-documentation options...
            if (String.Equals(category, Missing,
                StringComparison.CurrentCultureIgnoreCase))
            {
                if (_missingDocument != null)
                {
                    item = _missingDocument[option];
                }

                return item;
            }
            // 3. For the included-documentation options...
            if (String.Equals(category, Included,
                StringComparison.CurrentCultureIgnoreCase))
            {
                if (_includedDocument != null)
                {
                    item = _includedDocument[option];
                }

                return item;
            }

            return null;
        }

        #endregion

        #region Private Methods

        private void CreateContents()
        {   
            // 1. Create the option items for the auto-documentation options...
            CreateAutoDocContents();
            // 2. Create the option items for the missing documentation options...
            CreateMissingContents();
            // 3. Create the option items for the include documentation options...
            CreateIncludeContents();
        }

        private void CreateAutoDocContents()
        {
            // 1. Create the option items for the auto-documentation options...
            if (_autoDocument == null)
            {
                _autoDocument = new OptionContent("AutoDocumentation");
            }
            OptionItem item = null;
            item = new OptionItem("Constructors", true);
            _autoDocument.Add(item); 
            item = new OptionItem("Disposable", true);
            _autoDocument.Add(item);
        }

        private void CreateMissingContents()
        {
            OptionItem item = null;
            if (_missingDocument == null)
            {
                _missingDocument = new OptionContent("MissingDocumentation");
            }
            item = new OptionItem("Params",     true);
            _missingDocument.Add(item);
            item = new OptionItem("TypeParams", true);
            _missingDocument.Add(item);
            item = new OptionItem("Remarks",    false);
            _missingDocument.Add(item);
            item = new OptionItem("Returns",    true);
            _missingDocument.Add(item);
            item = new OptionItem("Summaries",  true);
            _missingDocument.Add(item);
            item = new OptionItem("Values",     false);
            _missingDocument.Add(item);
            item = new OptionItem("Namespaces", true);
            _missingDocument.Add(item);
        }

        private void CreateIncludeContents()
        {
            OptionItem item = null;
            if (_includedDocument == null)
            {
                _includedDocument = new OptionContent("IncludedDocumentation");
            }
            item = new OptionItem("Internals", false);
            _includedDocument.Add(item);
            item = new OptionItem("Versions", true);
            _includedDocument.Add(item);
        }

        #endregion

        #region ICloneable Members

        public override ReferenceOptions Clone()
        {
            ReferenceOptions refOptions = new ReferenceOptions(this);
            if (_autoDocument == null)
            {
                refOptions._autoDocument = _autoDocument.Clone();
            }
            if (_missingDocument == null)
            {
                refOptions._missingDocument = _missingDocument.Clone();
            }
            if (_includedDocument == null)
            {
                refOptions._includedDocument = _includedDocument.Clone();
            }

            return refOptions;
        }

        #endregion
    }
}
