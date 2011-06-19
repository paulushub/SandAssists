using System;
using System.Xml;
using System.Xml.XPath;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceXPathVisitor : ReferenceVisitor
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this visitor.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this visitor.
        /// </value>
        public const string VisitorName = 
            "Sandcastle.References.ReferenceXPathVisitor";

        #endregion

        #region Private Fields

        private ReferenceXPathConfiguration _xpathConfig;

        #endregion

        #region Constructors and Destructor

        public ReferenceXPathVisitor()
            : this((ReferenceXPathConfiguration)null)
        {   
        }

        public ReferenceXPathVisitor(ReferenceXPathConfiguration configuration)
            : base(VisitorName, configuration)
        {
            _xpathConfig = configuration;
        }

        public ReferenceXPathVisitor(ReferenceXPathVisitor source)
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
                return ReferenceXPathConfiguration.ConfigurationName;
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(BuildContext context, ReferenceGroup group)
        {
            base.Initialize(context, group);

            if (this.IsInitialized)
            {
                if (_xpathConfig == null)
                {
                    ReferenceEngineSettings engineSettings = this.EngineSettings;

                    Debug.Assert(engineSettings != null);
                    if (engineSettings == null)
                    {
                        this.IsInitialized = false;
                        return;
                    }

                    _xpathConfig = engineSettings.XPath;
                    Debug.Assert(_xpathConfig != null);
                    if (_xpathConfig == null)
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
            Debug.Assert(_xpathConfig != null);
            if (_xpathConfig == null || !_xpathConfig.Enabled)
            {
                return;
            }
            if (referenceDocument.DocumentType != ReferenceDocumentType.Reflection)
            {
                return;
            }

            XmlDocument xmlDocument = referenceDocument.Document;
            if (xmlDocument == null)
            {
                return;
            }

            XPathNavigator documentNavigator = xmlDocument.CreateNavigator();
            XPathNavigator rootNavigator     = documentNavigator.SelectSingleNode("reflection/apis");

            if (rootNavigator != null)
            {
                this.ApplyXPath(documentNavigator, rootNavigator);
            }
        }

        #endregion

        #region Private Methods

        #region ApplyXPath Method

        private void ApplyXPath(XPathNavigator documentNavigator, XPathNavigator rootNavigator)
        {
            IList<ReferenceXPathItem> xpathExpressions = _xpathConfig.Expressions;
            if (xpathExpressions == null || xpathExpressions.Count == 0)
            {
                return;
            }

            BuildContext context = this.Context;
            Debug.Assert(context != null);
            if (context == null)
            {
                return;
            }
            BuildLogger logger = context.Logger;
            if (logger != null)
            {
                logger.WriteLine("Begin Applying Path Document Visibility.",
                    BuildLoggerLevel.Info);
            }

            int itemCount = xpathExpressions.Count;
            for (int i = 0; i < itemCount; i++)
            {
                ReferenceXPathItem xpathExpression = xpathExpressions[i];

                if (xpathExpression != null && !xpathExpression.IsEmpty)
                {
                    XPathNodeIterator iterator = rootNavigator.Select(
                        xpathExpression.Expression);                
                    if (iterator != null && iterator.Count != 0)
                    {
                        string actionVerb = xpathExpression.Verb;
                        if (String.Equals(actionVerb, "DeleteSelf", 
                            StringComparison.OrdinalIgnoreCase))
                        {   
                            if (xpathExpression.UseApiNode)
                            {
                                this.ApplyDeleteVerb(rootNavigator,
                                    xpathExpression, logger);
                            }
                            else
                            {
                                this.ApplyDeleteVerb(documentNavigator,
                                    xpathExpression, logger);
                            }
                        }
                        else if (String.Equals(actionVerb, "SetValue", 
                            StringComparison.OrdinalIgnoreCase))
                        {
                            if (xpathExpression.UseApiNode)
                            {
                                this.ApplySetValueVerb(rootNavigator,
                                    xpathExpression, logger);
                            }
                            else
                            {
                                this.ApplySetValueVerb(documentNavigator,
                                    xpathExpression, logger);
                            }
                        }
                    }

                }
            }

            if (logger != null)
            {
                logger.WriteLine("Completed Applying Path Document Visibility.",
                    BuildLoggerLevel.Info);
            }
        }

        #endregion

        #region ApplyDeleteVerb Method

        private void ApplyDeleteVerb(XPathNavigator rootNavigator,
            ReferenceXPathItem xpathVisibility, BuildLogger logger)
        {
            if (!String.Equals(xpathVisibility.Verb, "DeleteSelf",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            XPathNodeIterator iterator = rootNavigator.Select(xpathVisibility.Expression);
            if (iterator == null || iterator.Count == 0)
            {
                return;
            }

            int itemCount = 0;

            XPathNavigator[] navigators = ToClonedArray(iterator);

            string condition = xpathVisibility.Condition;
            if (String.IsNullOrEmpty(condition))
            {
                foreach (XPathNavigator navigator in navigators)
                {
                    navigator.DeleteSelf();
                    itemCount++;
                }
            }
            else
            {
                XPathExpression xpathCondition = XPathExpression.Compile(condition);
                object evaluateResults = xpathVisibility.Results;

                foreach (XPathNavigator navigator in navigators)
                {
                    switch (xpathCondition.ReturnType)
                    {
                        case XPathResultType.Number:
                            if (evaluateResults != null && 
                                evaluateResults == navigator.Evaluate(xpathCondition))
                            {
                                navigator.DeleteSelf();
                                itemCount++;
                            }
                            break;

                        case XPathResultType.String:
                            if (evaluateResults != null && String.Equals((string)evaluateResults,
                                (string)navigator.Evaluate(xpathCondition), StringComparison.Ordinal))
                            {
                                navigator.DeleteSelf();
                                itemCount++;
                            }
                            break;

                        case XPathResultType.Boolean:
                            if (evaluateResults != null)
                            {
                                if (Convert.ToBoolean(evaluateResults) == (bool)navigator.Evaluate(xpathCondition))
                                {
                                    navigator.DeleteSelf();
                                    itemCount++;
                                }
                            }
                            else if ((bool)navigator.Evaluate(xpathCondition))
                            {
                                navigator.DeleteSelf();
                                itemCount++;
                            }
                            break;

                        case XPathResultType.NodeSet:
                            XPathNodeIterator selectIterator = navigator.Select(xpathCondition);
                            if (selectIterator != null && selectIterator.Count != 0)
                            {
                                XPathNavigator[] selectNavigators = ToClonedArray(selectIterator);
                                foreach (XPathNavigator selectNavigator in selectNavigators)
                                {
                                    selectNavigator.DeleteSelf();
                                    itemCount++;
                                }
                            }
                            break;
                    }   
                }
            }

            if (itemCount > 0 && logger != null)
            {
                logger.WriteLine(itemCount.ToString().PadRight(5) +
                    " Matched items removed.", BuildLoggerLevel.Info);
            }
        }

        #endregion

        #region ApplySetValueVerb Method

        private void ApplySetValueVerb(XPathNavigator rootNavigator,
            ReferenceXPathItem xpathVisibility, BuildLogger logger)
        {
            if (!String.Equals(xpathVisibility.Verb, "SetValue", 
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }
            string newValue = xpathVisibility.Value;
            if (newValue == null)
            {
                return;
            }

            XPathNodeIterator iterator = rootNavigator.Select(xpathVisibility.Expression);
            if (iterator == null || iterator.Count == 0)
            {
                return;
            }

            int itemCount = 0;

            string condition = xpathVisibility.Condition;
            string attribute = xpathVisibility.Attribute;
            if (String.IsNullOrEmpty(condition))
            {
                foreach (XPathNavigator navigator in iterator)
                {               
                    if (String.IsNullOrEmpty(attribute))
                    {
                        navigator.SetValue(newValue);
                        itemCount++;
                    }
                    else if (navigator.MoveToAttribute(attribute, String.Empty))
                    {
                        navigator.SetValue(newValue);
                        itemCount++;
                    }
                }
            }
            else
            {
                XPathExpression xpathCondition = XPathExpression.Compile(condition);
                object evaluateResults = xpathVisibility.Results;

                foreach (XPathNavigator navigator in iterator)
                {
                    switch (xpathCondition.ReturnType)
                    {
                        case XPathResultType.Number:
                            if (evaluateResults != null &&
                                evaluateResults == navigator.Evaluate(xpathCondition))
                            {
                                if (String.IsNullOrEmpty(attribute))
                                {
                                    navigator.SetValue(newValue);
                                    itemCount++;
                                }
                                else if (navigator.MoveToAttribute(attribute, String.Empty))
                                {
                                    navigator.SetValue(newValue);
                                    itemCount++;
                                }
                            }
                            break;

                        case XPathResultType.String:
                            if (evaluateResults != null && String.Equals((string)evaluateResults,
                                (string)navigator.Evaluate(xpathCondition), StringComparison.Ordinal))
                            {
                                if (String.IsNullOrEmpty(attribute))
                                {
                                    navigator.SetValue(newValue);
                                    itemCount++;
                                }
                                else if (navigator.MoveToAttribute(attribute, String.Empty))
                                {
                                    navigator.SetValue(newValue);
                                    itemCount++;
                                }
                            }
                            break;

                        case XPathResultType.Boolean:
                            if (evaluateResults != null)
                            {
                                if (Convert.ToBoolean(evaluateResults) == (bool)navigator.Evaluate(xpathCondition))
                                {
                                    if (String.IsNullOrEmpty(attribute))
                                    {
                                        navigator.SetValue(newValue);
                                        itemCount++;
                                    }
                                    else if (navigator.MoveToAttribute(attribute, String.Empty))
                                    {
                                        navigator.SetValue(newValue);
                                        itemCount++;
                                    }
                                }
                            }
                            else if ((bool)navigator.Evaluate(xpathCondition))
                            {
                                if (String.IsNullOrEmpty(attribute))
                                {
                                    navigator.SetValue(newValue);
                                    itemCount++;
                                }
                                else if (navigator.MoveToAttribute(attribute, String.Empty))
                                {
                                    navigator.SetValue(newValue);
                                    itemCount++;
                                }
                            }
                            break;

                        case XPathResultType.NodeSet:
                            XPathNodeIterator selectIterator = navigator.Select(xpathCondition);
                            if (selectIterator != null && selectIterator.Count != 0)
                            {
                                XPathNavigator[] selectNavigators = ToClonedArray(selectIterator);
                                foreach (XPathNavigator selectNavigator in selectNavigators)
                                {
                                    if (String.IsNullOrEmpty(attribute))
                                    {
                                        selectNavigator.SetValue(newValue);
                                        itemCount++;
                                    }
                                    else if (navigator.MoveToAttribute(attribute, String.Empty))
                                    {
                                        selectNavigator.SetValue(newValue);
                                        itemCount++;
                                    }
                                }
                            }
                            break;
                    }
                }
            }

            if (itemCount > 0 && logger != null)
            {
                logger.WriteLine(itemCount.ToString().PadRight(5) +
                    " Value of matched items changed.", BuildLoggerLevel.Info);
            }
        }

        #endregion

        #endregion

        #region ICloneable Members

        public override ReferenceVisitor Clone()
        {
            ReferenceXPathVisitor visitor = 
                new ReferenceXPathVisitor(this);

            return visitor;
        }

        #endregion
    }
}
