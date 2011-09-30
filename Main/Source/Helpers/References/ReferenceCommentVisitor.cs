using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceCommentVisitor : ReferenceVisitor
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this visitor.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this visitor.
        /// </value>
        public const string VisitorName =
            "Sandcastle.References.ReferenceCommentVisitor";

        #endregion

        #region Private Fields

        private static Regex _cppGenericFixRegex = new Regex("`[0-9]+(\\{)");

        private static XPathExpression _cppGenericExpression =
            XPathExpression.Compile("//member[starts-with(@name, 'M:')]/@name");

        private static XPathExpression _seeExpression =
            XPathExpression.Compile("//see[@topic or @dref]");

        private static XPathExpression _seealsoExpression =
            XPathExpression.Compile("//seealso[@topic or @dref]");

        private static XPathExpression _errorSeeExpression =
            XPathExpression.Compile("//see[starts-with(@cref, '!:') or starts-with(@cref, 'O:')]");

        private static XPathExpression _errorSeealsoExpression =
            XPathExpression.Compile("//seealso[starts-with(@cref, '!:') or starts-with(@cref, 'O:')]");

        private static XPathExpression _nsdocExpression = XPathExpression.Compile(
            "//member[starts-with(@name, 'T:') and contains(@name, '.NamespaceDoc')]");

        private ReferenceCommentConfiguration _comments;

        #endregion

        #region Constructors and Destructor

        public ReferenceCommentVisitor()
            : this((ReferenceCommentConfiguration)null)
        {
        }

        public ReferenceCommentVisitor(ReferenceCommentConfiguration configuration)
            : base(VisitorName, configuration)
        {
            _comments = configuration;
        }

        public ReferenceCommentVisitor(ReferenceCommentVisitor source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unique name of the target build configuration or options 
        /// processed by this reference visitor.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of the options processed by this
        /// reference visitor.
        /// </value>
        public override string TargetName
        {
            get
            {
                return ReferenceCommentConfiguration.ConfigurationName;
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(BuildContext context, ReferenceGroup group)
        {
            base.Initialize(context, group);

            if (this.IsInitialized)
            {
                if (_comments == null)
                {
                    ReferenceEngineSettings engineSettings = this.EngineSettings;

                    Debug.Assert(engineSettings != null);
                    if (engineSettings == null)
                    {
                        this.IsInitialized = false;
                        return;
                    }

                    _comments = engineSettings.Comments;
                    Debug.Assert(_comments != null);
                    if (_comments == null)
                    {
                        this.IsInitialized = false;
                        return;
                    }
                }
            }
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
        }

        public override void Visit(ReferenceDocument referenceDocument)
        {
            BuildExceptions.NotNull(referenceDocument, "referenceDocument");
            if (referenceDocument.DocumentType != ReferenceDocumentType.Comments)
            {
                return;
            }

            BuildContext context = this.Context;
            Debug.Assert(context != null);
            if (context == null)
            {
                return;
            }

            this.OnVisit(referenceDocument, context);
        }

        #endregion

        #region Private Methods

        #region OnVisit Method

        private void OnVisit(ReferenceDocument referenceDocument, BuildContext context)
        {
            BuildLogger logger = context.Logger;

            string fileName = Path.GetFileName(referenceDocument.DocumentFile);

            if (logger != null)
            {
                logger.WriteLine("Begin Fixing Comment: " + fileName,
                    BuildLoggerLevel.Info);
            }

            XmlDocument document = referenceDocument.Document;
            XPathNavigator documentNavigator = document.CreateNavigator();

            // For the NamespaceDoc classes...
            if (_comments.UsesNamespaceDoc)
            {
                int itemCount = 0;

                XPathNodeIterator iterator = documentNavigator.Select(
                    _nsdocExpression);

                if (iterator != null && iterator.Count != 0)
                {
                    itemCount = iterator.Count;

                    foreach (XPathNavigator navigator in iterator)
                    {
                        string nameText = navigator.GetAttribute("name", String.Empty);

                        // Change from: T:Company.TestLibrary.NamespaceDoc
                        //        to:   N:Company.TestLibrary
                        if (navigator.MoveToAttribute("name", String.Empty))
                        {
                            // 15 := sizeof(.NamespaceDoc) + sizeof(T:)
                            navigator.SetValue(
                                "N:" + nameText.Substring(2, nameText.Length - 15));
                        }
                    }
                }   

                if (logger != null)
                {
                    logger.WriteLine("Total number of 'NamespaceDoc' found: " +
                        itemCount.ToString(), BuildLoggerLevel.Info);
                }
            }   

            if (_comments.FixComments)
            {
                int itemFound = 0;
                int itemFixed = 0;

                bool conceptualTopics = false;

                if (context.Settings.BuildConceptual)
                {
                    IList<BuildGroupContext> groupContexts = context.GroupContexts;
                    for (int i = 0; i < groupContexts.Count; i++)
                    {
                        BuildGroupContext groupContext = groupContexts[i];
                        if (groupContext.GroupType == BuildGroupType.Conceptual)
                        {
                            conceptualTopics = true;
                            break;
                        }
                    }
                }           

                KeyValuePair<int, int> errorSeeCount = this.OnFixErrorLinks(
                    documentNavigator, _errorSeeExpression, logger);
                itemFound += errorSeeCount.Key;
                itemFixed += errorSeeCount.Value;

                KeyValuePair<int, int> errorSeealsoCount = this.OnFixErrorLinks(
                    documentNavigator, _errorSeealsoExpression, logger);
                itemFound += errorSeealsoCount.Key;
                itemFixed += errorSeealsoCount.Value;

                // Counting this fixes is time wasting, we might have to
                // compare the input and output texts
                XPathNodeIterator it = documentNavigator.Select(_cppGenericExpression);
                while (it.MoveNext())
                {
                    string methodName = _cppGenericFixRegex.Replace(
                        it.Current.Value, "$1");

                    it.Current.SetValue(methodName);
                }

                if (conceptualTopics)
                {
                    KeyValuePair<int, int> seeCount = this.OnFixTopicLinks(
                        documentNavigator, _seeExpression, logger);
                    itemFound += seeCount.Key;
                    itemFixed += seeCount.Value;

                    KeyValuePair<int, int> seealsoCount = this.OnFixTopicLinks(
                        documentNavigator, _seealsoExpression, logger);
                    itemFound += seealsoCount.Key;
                    itemFixed += seealsoCount.Value;
                }

                if (logger != null)
                {
                    logger.WriteLine(String.Format(
                        "Total number of link issues fixed: {0} out of {1}",
                        itemFixed, itemFound), BuildLoggerLevel.Info);
                }
            }

            if (logger != null)
            {
                logger.WriteLine("Completed Fixing Comment: " + fileName, 
                    BuildLoggerLevel.Info);
            }
        }

        private KeyValuePair<int, int> OnFixErrorLinks(XPathNavigator documentNavigator, 
            XPathExpression expression, BuildLogger logger)
        {
            int itemFound = 0;
            int itemFixed = 0;

            XPathNodeIterator iterator = documentNavigator.Select(
                expression);   
            
            if (iterator != null && iterator.Count != 0)
            {
                itemFound = iterator.Count;

                string itemTypes = "EFMNPT";

                foreach (XPathNavigator navigator in iterator)
                {
                    string crefError = navigator.GetAttribute("cref", String.Empty);
                    if (crefError.Length <= 4)
                    {
                        continue;
                    }

                    // General: cref="!:T:TestLibraryCLR.PointBase"
                    // Overloads: cref="!:Overload:TestLibraryCLR.PointBase.TestMethod"
                    //            cref="O:TestLibraryCLR.PointBase.TestMethod"
                    if (crefError.StartsWith("O:", StringComparison.OrdinalIgnoreCase))
                    {
                        string nameText = "Overload" + crefError.Substring(1);
                        if (navigator.MoveToAttribute("cref", String.Empty))
                        {
                            navigator.SetValue(nameText);
                            itemFixed++;
                        }
                    }
                    else if (crefError.StartsWith("!:O:", StringComparison.OrdinalIgnoreCase))
                    {
                        string nameText = "Overload" + crefError.Substring(3);
                        if (navigator.MoveToAttribute("cref", String.Empty))
                        {
                            navigator.SetValue(nameText);
                            itemFixed++;
                        }
                    }
                    else
                    {
                        string crefText = crefError.Substring(2);
                        if (crefText[1] == ':' && itemTypes.IndexOf(crefText[0]) >= 0
                            || crefText.StartsWith("Overload", StringComparison.Ordinal))
                        {
                            if (navigator.MoveToAttribute("cref", String.Empty))
                            {
                                navigator.SetValue(crefText);
                                itemFixed++;
                            }
                        }
                    }
                }
            }   

            return new KeyValuePair<int, int>(itemFound, itemFixed);
        }

        private KeyValuePair<int, int> OnFixTopicLinks(XPathNavigator documentNavigator, 
            XPathExpression expression, BuildLogger logger)
        {
            int itemFound = 0;
            int itemFixed = 0;

            XPathNodeIterator iterator = documentNavigator.Select(expression);
            if (iterator != null && iterator.Count != 0)
            {
                itemFound = iterator.Count;

                foreach (XPathNavigator navigator in iterator)
                {
                    string topicId = navigator.GetAttribute("topic", String.Empty);
                    if (!String.IsNullOrEmpty(topicId))
                    {
                        navigator.CreateAttribute(navigator.Prefix, "cref",
                            navigator.NamespaceURI, topicId);

                        itemFixed++;
                    }
                }
            }

            return new KeyValuePair<int, int>(itemFound, itemFixed);
        }

        #endregion

        #endregion

        #region ICloneable Members

        public override ReferenceVisitor Clone()
        {
            ReferenceCommentVisitor visitor = new ReferenceCommentVisitor(this);

            return visitor;
        }

        #endregion
    }
}
