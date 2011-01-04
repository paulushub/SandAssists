﻿using System;
using System.Xml;
using System.Collections.Generic;

using Sandcastle.Contents;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceCodeConfiguration : ReferenceComponentConfiguration
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this configuration.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this configuration.
        /// </value>
        public const string ConfigurationName =
            "Sandcastle.References.ReferenceCodeConfiguration";

        #endregion

        #region Private Fields

        private int    _tabSize;
        private bool   _showLineNumbers;
        private bool   _showOutlining;
        private string _highlightMode;

        private string _snippetSeparator;
        private BuildCacheStorageType _snippetStorage;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceCodeConfiguration"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceCodeConfiguration"/> class
        /// to the default values.
        /// </summary>
        public ReferenceCodeConfiguration()
            : this(ConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceCodeConfiguration"/> class
        /// with the specified options or category name.
        /// </summary>
        /// <param name="optionsName">
        /// A <see cref="System.String"/> specifying the name of this category of options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="optionsName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="optionsName"/> is empty.
        /// </exception>
        private ReferenceCodeConfiguration(string optionsName)
            : base(optionsName)
        {
            _tabSize          = 4;
            _showLineNumbers  = false;
            _showOutlining    = false;
            _highlightMode    = "IndirectIris";
            _snippetSeparator = "...";
            _snippetStorage   = BuildCacheStorageType.Database;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceCodeConfiguration"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceCodeConfiguration"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceCodeConfiguration"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceCodeConfiguration(ReferenceCodeConfiguration source)
            : base(source)
        {
            _tabSize          = source._tabSize;
            _showLineNumbers  = source._showLineNumbers;
            _showOutlining    = source._showOutlining;
            _highlightMode    = source._highlightMode;
            _snippetStorage   = source._snippetStorage;
            _snippetSeparator = source._snippetSeparator;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value specifying whether this options category is active, and should
        /// be process.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if this options category enabled and userable 
        /// in the build process; otherwise, it is <see langword="false"/>.
        /// </value>
        public override bool IsActive
        {
            get
            {
                return base.IsActive;
            }
        }

        /// <summary>
        /// Gets the source of the build component supported by this configuration.
        /// </summary>
        /// <value>
        /// An enumeration of the type, <see cref="BuildComponentType"/>,
        /// specifying the source of this build component.
        /// </value>
        public override BuildComponentType ComponentType
        {
            get
            {
                return BuildComponentType.SandcastleAssist;
            }
        }

        /// <summary>
        /// Gets the unique name of the build component supported by this configuration. 
        /// </summary>
        /// <value>
        /// A string containing the unique name of the build component, this 
        /// should normally include the namespace.
        /// </value>
        public override string ComponentName
        {
            get
            {
                return "Sandcastle.Components.ReferenceCodeComponent";
            }
        }

        /// <summary>
        /// Gets the path of the build component supported by this configuration.
        /// </summary>
        /// <value>
        /// A string containing the path to the assembly in which the build
        /// component is defined.
        /// </value>
        public override string ComponentPath
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a value specifying whether this configuration is displayed or 
        /// visible to the user.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if this configuration is visible
        /// and accessible to the user; otherwise it is <see langword="false"/>.
        /// </value>
        public override bool IsBrowsable
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a copyright and license notification for the component targeted 
        /// by this configuration.
        /// </summary>
        /// <value>
        /// A string specifying the copyright and license of the component.
        /// </value>
        public override string Copyright
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the description of the component targeted by this configuration.
        /// </summary>
        /// <value>
        /// A string providing a description of the component.
        /// </value>
        /// <remarks>
        /// This must be a plain text, brief and informative.
        /// </remarks>
        public override string Description
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the file name of the documentation explaining the features and
        /// how to use the component.
        /// </summary>
        /// <value>
        /// A string containing the file name of the documentation.
        /// </value>
        /// <remarks>
        /// <para>
        /// This should be either a file name (with file extension, but without
        /// the path) or include a path relative to the assembly containing this
        /// object implementation.
        /// </para>
        /// <para>
        /// The expected file format is HTML, PDF, XPS, CHM and plain text.
        /// </para>
        /// </remarks>
        public override string HelpFileName
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the version of the target component.
        /// </summary>
        /// <value>
        /// An instance of <see cref="System.Version"/> specifying the version
        /// of the target component.
        /// </value>
        public override Version Version
        {
            get
            {
                return new Version(1, 0, 0, 0);
            }
        }

        public int TabSize
        {
            get
            {
                return _tabSize;
            }
            set
            {
                 if (value > 0)
                 {
                     _tabSize = value;
                 }
            }
        }

        public bool ShowLineNumbers
        {
            get
            {
                return _showLineNumbers;
            }
            set
            {
                _showLineNumbers = value;
            }
        }

        public string SnippetSeparator
        {
            get
            {
                return _snippetSeparator;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }

                if (!String.IsNullOrEmpty(value))
                {
                    _snippetSeparator = value;
                }
            }
        }

        public BuildCacheStorageType SnippetStorage
        {
            get
            {
                return _snippetStorage;
            }
            set
            {
                _snippetStorage = value;
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(BuildContext context)
        {
            base.Initialize(context);
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
        }

        /// <summary>
        /// The creates the configuration information or settings required by the
        /// target component for the build process.
        /// </summary>
        /// <param name="group">
        /// A build group, <see cref="BuildGroup"/>, representing the documentation
        /// target for configuration.
        /// </param>
        /// <param name="writer">
        /// An <see cref="XmlWriter"/> object used to create one or more new 
        /// child nodes at the end of the list of child nodes of the current node. 
        /// </param>
        /// <returns>
        /// Returns <see langword="true"/> for a successful configuration;
        /// otherwise, it returns <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// The <see cref="XmlWriter"/> writer passed to this configuration object
        /// may be passed on to other configuration objects, so do not close or 
        /// dispose it.
        /// </remarks>
        public override bool Configure(BuildGroup group, XmlWriter writer)
        {
            BuildExceptions.NotNull(group, "group");
            BuildExceptions.NotNull(writer, "writer");

            if (!this.Enabled)
            {
                return false;
            }

            //<component type="Sandcastle.Components.ReferenceCodeComponent" assembly="$(SandAssistComponent)">
            //    <options mode="IndirectIris" tabSize="4" lineNumbers="true" outlining="false"/>
            //
            //    <!--The following options are for processing codeReference tags in the 
            //    reference help.
            //    It is a replacement of the ExampleComponent, providing better coloring,
            //    minimum memory usage etc.
            //  
            //    $codeSnippets   
            //      @process: * Indicates whether or not to process the codeReference.
            //                * If true, the codeReference tags are process by this component
            //                * Default: false
            //             
            //      @storage: * Indicates where the code snippets should be stored after loading
            //                * Possible values are
            //                     - Memory: the snippets are stored in memory similar to 
            //                               the ExampleComponent.
            //                     - Database: the snippets are stored in Sqlite database.
            //                * Default: Database 
            //      @separator: * For multi-parts snippets, this defines the separator... 
            //                  * Default: ...-->
            //
            //    <!--<codeSnippets process="true" storage="Sqlite" separator="...">
            //        <codeSnippet source=".\CodeSnippetSample.xml" format="Sandcastle" />
            //    </codeSnippets>-->
            //    <SandcastleItem name="%CodeSnippets%" />
            //</component>
                                                  
            writer.WriteStartElement("options");  //start: options
            writer.WriteAttributeString("mode", _highlightMode);
            writer.WriteAttributeString("tabSize", _tabSize.ToString());
            writer.WriteAttributeString("lineNumbers", _showLineNumbers.ToString());
            writer.WriteAttributeString("outlining", _showOutlining.ToString());
            writer.WriteEndElement();             //end: options

            IList<SnippetContent> listSnippets = group.SnippetContents;
            if (listSnippets != null && listSnippets.Count != 0)
            {
                writer.WriteStartElement("codeSnippets");  // start - codeSnippets
                writer.WriteAttributeString("process", "true");
                writer.WriteAttributeString("storage", _snippetStorage.ToString());
                writer.WriteAttributeString("separator", _snippetSeparator);

                int contentCount = listSnippets.Count;
                for (int i = 0; i < contentCount; i++)
                {
                    SnippetContent snippetContent = listSnippets[i];
                    if (snippetContent == null || snippetContent.IsEmpty)
                    {
                        continue;
                    }
                    writer.WriteStartElement("codeSnippet"); // start - codeSnippet
                    writer.WriteAttributeString("source", snippetContent.ContentsFile);
                    writer.WriteAttributeString("format", "Sandcastle");
                    writer.WriteEndElement();                // end - codeSnippet
                }

                writer.WriteEndElement();                  // end - codeSnippets
            }

            return true;
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// This creates a new build object that is a deep copy of the current 
        /// instance.
        /// </summary>
        /// <returns>
        /// A new build object that is a deep copy of this instance.
        /// </returns>
        public override BuildComponentConfiguration Clone()
        {
            ReferenceCodeConfiguration options = new ReferenceCodeConfiguration(this);

            return options;
        }

        #endregion
    }
}
