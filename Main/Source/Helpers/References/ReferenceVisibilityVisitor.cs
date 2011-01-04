using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceVisibilityVisitor : ReferenceVisitor
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this visitor.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this visitor.
        /// </value>
        public const string VisitorName =
            "Sandcastle.References.ReferenceVisibilityVisitor";

        #endregion

        #region Private Fields

        private ReferenceVisibilityConfiguration _visibility;

        #endregion

        #region Constructors and Destructor

        public ReferenceVisibilityVisitor()
            : this((ReferenceEngineSettings)null)
        {   
        }

        public ReferenceVisibilityVisitor(ReferenceEngineSettings engineSettings)
            : base(VisitorName, engineSettings)
        {
        }

        public ReferenceVisibilityVisitor(ReferenceVisibilityVisitor source)
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
                return ReferenceVisibilityConfiguration.ConfigurationName;
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(BuildContext context, ReferenceGroup group)
        {
            base.Initialize(context, group);

            if (this.IsInitialized)
            {
                ReferenceEngineSettings engineSettings = this.EngineSettings;

                Debug.Assert(engineSettings != null);
                if (engineSettings == null)
                {
                    this.IsInitialized = false;
                    return;
                }

                _visibility = engineSettings.Visibility;
                Debug.Assert(_visibility != null);
                if (_visibility == null)
                {
                    this.IsInitialized = false;
                    return;
                }
            }
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
        }

        public override void Visit(ReferenceDocument refDocument)
        {
            BuildExceptions.NotNull(refDocument, "refDocument");
            if (refDocument.DocumentType != ReferenceDocumentType.Reflection)
            {
                return;
            }

            XmlDocument xmlDocument = refDocument.Document;
            if (xmlDocument == null)
            {
                return;
            }
            if (_visibility == null || !_visibility.Enabled)
            {
                return;
            }

            XPathNavigator documentNavigator = xmlDocument.CreateNavigator();
            XPathNavigator rootNavigator     = documentNavigator.SelectSingleNode(
                "reflection/apis");

            if (rootNavigator != null)
            {
                this.ApplyVisibility(rootNavigator);
            }
        }

        #endregion

        #region Private Methods

        #region ApplyVisibility Method

        private void ApplyVisibility(XPathNavigator rootNavigator)
        {
            BuildContext context = this.Context;
            Debug.Assert(context != null);
            if (context == null)
            {
                return;
            }
            BuildLogger logger = context.Logger;
            if (logger != null)
            {
                logger.WriteLine("Begin Applying Document Visibility.",
                    BuildLoggerLevel.Info);
            }

            if (!_visibility.AttributeInformation)
            {
                this.ApplyAttributesVisibility(rootNavigator, logger);
            }
            if (!_visibility.ExplicitInterfaceMembers)
            {
                this.ApplyExplicitInterfaceVisibility(rootNavigator, logger);
            }
            if (!_visibility.ExcludeInheritedMembers)
            {
                this.ApplyInheritedMembersVisibility(rootNavigator, logger);
            }
            
            if (!_visibility.PrivateMembers)
            {
                this.ApplyPrivateMembersVisibility(rootNavigator, logger);
            }
            else if (!_visibility.PrivateFields)
            {
                this.ApplyPrivateFieldsVisibility(rootNavigator, logger);
            }
            
            if (!_visibility.InternalMembers)
            {
                this.ApplyInternalMembersVisibility(rootNavigator, logger);
            }

            if (!_visibility.ProtectedMembers)
            {
                this.ApplyProtectedMembersVisibility(rootNavigator, logger);
            }
            else
            {
                if (_visibility.ProtectedInternalsAsProtectedMembers)
                {
                    this.ApplyProtectedInternalMembersVisibility(
                        rootNavigator, logger);
                }
                if (!_visibility.SealedProtectedMembers)
                {
                    this.ApplySealedProtectedMembersVisibility(
                        rootNavigator, logger);
                }
            }

            // This should be the last to be applied...
            if (!_visibility.EmptyNamespaces)
            {
                this.ApplyEmptyNamespacesVisibility(rootNavigator, logger);
            }

            if (logger != null)
            {
                logger.WriteLine("Completed Applying Document Visibility.",
                    BuildLoggerLevel.Info);
            }
        }

        #endregion

        #region ApplyAttributesVisibility Method

        private void ApplyAttributesVisibility(
            XPathNavigator rootNavigator, BuildLogger logger)
        {
            if (_visibility.AttributeInformation)
            {
                return;
            }

            XPathNodeIterator iterator = rootNavigator.Select("api/attributes/attribute");
            if (iterator == null || iterator.Count == 0)
            {
                return;
            }

            XPathNavigator[] navigators = ToClonedArray(iterator);

            int itemCount = 0;

            foreach (XPathNavigator navigator in navigators)
            {
                XPathNavigator typeNavigator = navigator.SelectSingleNode("type");

                if (typeNavigator != null)
                {
                    string attributeApi = typeNavigator.GetAttribute("api", String.Empty);
                    if (typeNavigator != null && !_visibility.IsAttributeKept(attributeApi))
                    {
                        navigator.DeleteSelf();
                        itemCount++;
                    }
                }
            }

            if (itemCount > 0 && logger != null)
            {
                logger.WriteLine(itemCount.ToString().PadRight(5) +
                    " Attributes removed.", BuildLoggerLevel.Info);
            }
        }

        #endregion

        #region ApplyExplicitInterfaceVisibility Method

        private void ApplyExplicitInterfaceVisibility(
            XPathNavigator rootNavigator, BuildLogger logger)
        {
            if (_visibility.ExplicitInterfaceMembers)
            {
                return;
            }

            int itemCount = 0;

            XPathNodeIterator iterator = rootNavigator.Select(
                "api[memberdata/@visibility='private' and proceduredata/@virtual='true']");
            if (iterator != null && iterator.Count != 0)
            {
                XPathNavigator[] navigators = ToClonedArray(iterator);

                foreach (XPathNavigator navigator in navigators)
                {
                    navigator.DeleteSelf();
                    itemCount++;
                }
            }

            iterator = rootNavigator.Select(
                "api/elements/element[memberdata/@visibility='private' and proceduredata/@virtual='true']");
            if (iterator != null && iterator.Count != 0)
            {
                XPathNavigator[] navigators = ToClonedArray(iterator);

                foreach (XPathNavigator navigator in navigators)
                {
                    navigator.DeleteSelf();
                    itemCount++;
                }
            }  

            if (itemCount > 0 && logger != null)
            {
                logger.WriteLine(itemCount.ToString().PadRight(5) + 
                    " Explicit interface implementations removed.", BuildLoggerLevel.Info);
            }
        }

        #endregion

        #region ApplyPrivateFieldsVisibility Method

        private void ApplyPrivateFieldsVisibility(
            XPathNavigator rootNavigator, BuildLogger logger)
        {
            if (_visibility.PrivateFields)
            {
                return;
            }

            int itemCount = 0;

            // Apply the private fields...
            XPathNodeIterator iterator = rootNavigator.Select(
                "api[apidata/@subgroup='field' and memberdata/@visibility='private']");
            if (iterator != null && iterator.Count != 0)
            {
                XPathNavigator[] navigators = ToClonedArray(iterator);

                foreach (XPathNavigator navigator in navigators)
                {
                    navigator.DeleteSelf();
                    itemCount++;
                }
            }

            // Apply the framework private fields...
            iterator = rootNavigator.Select(
                "api/elements/element[starts-with(@api, 'F:System.') or starts-with(@api, 'F:Microsoft.')]");
            if (iterator != null && iterator.Count != 0)
            {
                XPathNavigator[] navigators = ToClonedArray(iterator);

                foreach (XPathNavigator navigator in navigators)
                {
                    navigator.DeleteSelf();
                    itemCount++;
                }
            }

            if (itemCount > 0 && logger != null)
            {
                logger.WriteLine(itemCount.ToString().PadRight(5) +
                    " Private fields removed.", BuildLoggerLevel.Info);
            }
        }

        #endregion

        #region ApplyPrivateMembersVisibility Method

        private void ApplyPrivateMembersVisibility(
            XPathNavigator rootNavigator, BuildLogger logger)
        {
            if (_visibility.PrivateMembers)
            {
                return;
            }

            int itemCount = 0;

            // Apply the private members...
            XPathNodeIterator iterator = rootNavigator.Select(
                "api[memberdata/@visibility='private' and (not(proceduredata) or proceduredata/@virtual='false')]");
            if (iterator != null && iterator.Count != 0)
            {
                XPathNavigator[] navigators = ToClonedArray(iterator);

                foreach (XPathNavigator navigator in navigators)
                {
                    navigator.DeleteSelf();
                    itemCount++;
                }
            }

            // <element api="M:System.Object.FieldSetter(System.String,System.String,System.Object)"> etc...
            iterator = rootNavigator.Select(
                "api/elements/element[memberdata/@visibility='private' and (not(proceduredata) or proceduredata/@virtual='false')]");
            if (iterator != null && iterator.Count != 0)
            {
                XPathNavigator[] navigators = ToClonedArray(iterator);

                foreach (XPathNavigator navigator in navigators)
                {
                    navigator.DeleteSelf();
                    itemCount++;
                }
            }

            // Apply the private types...
            iterator = rootNavigator.Select("api[typedata/@visibility='private']");
            if (iterator != null && iterator.Count != 0)
            {
                XPathNavigator[] navigators = ToClonedArray(iterator);

                foreach (XPathNavigator navigator in navigators)
                {
                    navigator.DeleteSelf();
                    itemCount++;
                }
            }

            if (itemCount > 0 && logger != null)
            {
                logger.WriteLine(itemCount.ToString().PadRight(5) +
                    " Private members removed.", BuildLoggerLevel.Info);
            }
        }

        #endregion

        #region ApplyInternalMembersVisibility Method

        private void ApplyInternalMembersVisibility(
            XPathNavigator rootNavigator, BuildLogger logger)
        {
            if (_visibility.InternalMembers)
            {
                return;
            }

            int itemCount = 0;

            // Apply the protected internal and internal members...
            XPathNodeIterator iterator = rootNavigator.Select(
                "api[memberdata/@visibility='assembly' or memberdata/@visibility='family or assembly']");
            if (iterator != null && iterator.Count != 0)
            {
                XPathNavigator[] navigators = ToClonedArray(iterator);

                foreach (XPathNavigator navigator in navigators)
                {
                    navigator.DeleteSelf();
                    itemCount++;
                }
            }

            // Apply the internal types...
            iterator = rootNavigator.Select(
                "api[typedata/@visibility='assembly' or typedata/@visibility='family or assembly']");
            if (iterator != null && iterator.Count != 0)
            {
                XPathNavigator[] navigators = ToClonedArray(iterator);

                foreach (XPathNavigator navigator in navigators)
                {
                    navigator.DeleteSelf();
                    itemCount++;
                }
            }

            if (itemCount > 0 && logger != null)
            {
                logger.WriteLine(itemCount.ToString().PadRight(5) +
                    " Internal members removed.", BuildLoggerLevel.Info);
            }
        }

        #endregion

        #region ApplyProtectedMembersVisibility Method

        private void ApplyProtectedMembersVisibility(
            XPathNavigator rootNavigator, BuildLogger logger)
        {
            if (_visibility.ProtectedMembers)
            {
                return;
            }

            int itemCount = 0;

            // Apply the protected members...
            XPathNodeIterator iterator = rootNavigator.Select(
                "api[memberdata/@visibility='family']");
            if (iterator != null && iterator.Count != 0)
            {
                XPathNavigator[] navigators = ToClonedArray(iterator);

                foreach (XPathNavigator navigator in navigators)
                {
                    navigator.DeleteSelf();
                    itemCount++;
                }
            }

            // Apply the framework protected inherited members...
            iterator = rootNavigator.Select(
                "api/elements/element[(starts-with(substring-after(@api,':'),'System.') or starts-with(substring-after(@api,':'),'Microsoft.')) and (memberdata/@visibility='family')]");
            if (iterator != null && iterator.Count != 0)
            {
                XPathNavigator[] navigators = ToClonedArray(iterator);

                foreach (XPathNavigator navigator in navigators)
                {
                    navigator.DeleteSelf();
                    itemCount++;
                }
            }

            if (itemCount > 0 && logger != null)
            {
                logger.WriteLine(itemCount.ToString().PadRight(5) +
                    " Protected members removed.", BuildLoggerLevel.Info);
            }
        }

        #endregion

        #region ApplyProtectedInternalMembersVisibility Method

        private void ApplyProtectedInternalMembersVisibility(
            XPathNavigator rootNavigator, BuildLogger logger)
        {
            if (!_visibility.ProtectedInternalsAsProtectedMembers)
            {
                return;
            }

            int itemCount = 0;

            // Apply the protected members...
            XPathNodeIterator iterator = rootNavigator.Select(
                "api/memberdata[@visibility='family or assembly']");
            if (iterator != null && iterator.Count != 0)
            {
                foreach (XPathNavigator navigator in iterator)
                {
                    if (navigator.MoveToAttribute("visibility", String.Empty))
                    {
                        navigator.SetValue("family");
                        itemCount++;
                    }
                }
            }

            if (itemCount > 0 && logger != null)
            {
                logger.WriteLine(itemCount.ToString().PadRight(5) +
                    " Protected internals documented as protected members.", BuildLoggerLevel.Info);
            }
        }

        #endregion

        #region ApplySealedProtectedMembersVisibility Method

        private void ApplySealedProtectedMembersVisibility(
            XPathNavigator rootNavigator, BuildLogger logger)
        {
            if (_visibility.SealedProtectedMembers)
            {
                return;
            }

            int itemCount = 0;

            // Apply the protected members...
            XPathNodeIterator iterator = rootNavigator.Select(
                "api[memberdata/@visibility='family' and proceduredata/@virtual='true' and proceduredata/@final='true']");
            if (iterator != null && iterator.Count != 0)
            {
                XPathNavigator[] navigators = ToClonedArray(iterator);

                foreach (XPathNavigator navigator in navigators)
                {
                    string apiId = navigator.GetAttribute("id", String.Empty);
                    if (apiId != null && apiId.Length > 2 && 
                        (apiId[0] == 'P' || apiId[0] == 'M' && apiId[1] == ':'))
                    {
                        navigator.DeleteSelf();
                        itemCount++;
                    }
                }
            }

            if (itemCount > 0 && logger != null)
            {
                logger.WriteLine(itemCount.ToString().PadRight(5) +
                    " Protected sealed members removed.", BuildLoggerLevel.Info);
            }
        }

        #endregion

        #region ApplyInheritedMembersVisibility Method

        private void ApplyInheritedMembersVisibility(
            XPathNavigator rootNavigator, BuildLogger logger)
        {
            if (_visibility.ExcludeInheritedMembers)
            {
                return;
            }

            bool inheritedMembers                  = !_visibility.InheritedMembers; 
            bool frameworkInheritedMembers         = !_visibility.FrameworkInheritedMembers;
            bool frameworkInheritedInternalMembers = !_visibility.FrameworkInheritedPrivateMembers;
            bool frameworkInheritedPrivateMembers  = !_visibility.FrameworkInheritedPrivateMembers;

            // Select all the reference types: classes and interfaces...
            XPathNodeIterator iterator = rootNavigator.Select(
                "api[starts-with(@id, 'T:') and (apidata/@subgroup='interface' or apidata/@subgroup='class')]");
            if (iterator == null || iterator.Count == 0)
            {
                return;
            }

            string frameworkSelector = String.Empty;
            if (!inheritedMembers)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(
                    "elements/element[(starts-with(substring-after(@api,':'),'System.') or starts-with(substring-after(@api,':'),'Microsoft.'))");

                if (!frameworkInheritedMembers)
                {
                    if (frameworkInheritedInternalMembers && frameworkInheritedPrivateMembers)
                    {
                        builder.Append(
                            " and (memberdata/@visibility='private' or memberdata/@visibility='assembly' or memberdata/@visibility='family or assembly')");
                    }
                    else
                    {
                        if (frameworkInheritedPrivateMembers)
                        {
                            builder.Append(
                                " and memberdata/@visibility='private'");
                        }
                        else
                        {
                            builder.Append(
                                " and (memberdata/@visibility='assembly' or memberdata/@visibility='family or assembly')");
                        }
                    }
                }

                builder.Append("]");
                frameworkSelector = builder.ToString();   
            }

            int itemCount = 0;

            foreach (XPathNavigator navigator in iterator)
            {
                if (inheritedMembers)
                {
                    // <api id="T:ANamespace.IAClass">
                    string typeId = navigator.GetAttribute("id", String.Empty);
                    if (typeId.Length > 2)
                    {
                        //<elements>
                        //    <element api="M:ANamespace.IAClass.ToString" />
                        //    <element api="P:ANamespace.IBClass.Text" />
                        //</elements> 
                        XPathNodeIterator inheritedIterator = navigator.Select(
                            "elements/element[not(starts-with(substring-after(@api,':'),'" + typeId.Substring(2) + "'))]");

                        if (inheritedIterator != null && inheritedIterator.Count != 0)
                        {
                            XPathNavigator[] inheritedNavigators = ToClonedArray(inheritedIterator);

                            foreach (XPathNavigator inheritedNavigator in inheritedNavigators)
                            {
                                inheritedNavigator.DeleteSelf();
                                itemCount++;
                            }
                        }
                    }
                }
                else
                {
                    XPathNodeIterator inheritedIterator = navigator.Select(frameworkSelector);

                    if (inheritedIterator != null && inheritedIterator.Count != 0)
                    {
                        XPathNavigator[] inheritedNavigators = ToClonedArray(inheritedIterator);

                        foreach (XPathNavigator inheritedNavigator in inheritedNavigators)
                        {
                            inheritedNavigator.DeleteSelf();
                            itemCount++;
                        }
                    }
                }
            }

            if (itemCount > 0 && logger != null)
            {
                logger.WriteLine(itemCount.ToString().PadRight(5) +
                    " Inherited members removed.", BuildLoggerLevel.Info);
            }
        }

        #endregion

        #region ApplyEmptyNamespacesVisibility Method

        private void ApplyEmptyNamespacesVisibility(
            XPathNavigator rootNavigator, BuildLogger logger)
        {
            if (_visibility.EmptyNamespaces)
            {
                return;
            }

            XPathNodeIterator iterator = rootNavigator.Select("api[starts-with(@id, 'N:')]");
            if (iterator == null || iterator.Count == 0)
            {
                return;
            }

            List<XPathNavigator> navigators = new List<XPathNavigator>(iterator.Count);

            foreach (XPathNavigator navigator in iterator)
            {
                XPathNodeIterator elementIterator = navigator.Select("elements/element");
                if (elementIterator == null || elementIterator.Count == 0)
                {
                    navigators.Add(navigator.Clone());
                }
                else
                {
                    bool isFound = false;
                    foreach (XPathNavigator elementNavigator in elementIterator)
                    {
                        string elementId = elementNavigator.GetAttribute("api", String.Empty);
                        if (!String.IsNullOrEmpty(elementId))
                        {
                            XPathNavigator typeNavigator = rootNavigator.SelectSingleNode(
                                "api[@id='" + elementId + "']");
                            if (typeNavigator != null)
                            {
                                isFound = true;
                                break;
                            }
                        }
                    }
                    if (!isFound)
                    {
                        navigators.Add(navigator.Clone());
                    }
                }   
            }

            int itemCount = navigators.Count;
            if (itemCount != 0)
            {
                for (int i = 0; i < itemCount; i++)
                {
                    navigators[i].DeleteSelf();
                }
            }

            if (itemCount > 0 && logger != null)
            {
                logger.WriteLine(itemCount.ToString().PadRight(5) +
                    " Empty namespaces removed.", BuildLoggerLevel.Info);
            }
        }

        #endregion

        #endregion

        #region ICloneable Members

        public override ReferenceVisitor Clone()
        {
            ReferenceVisibilityVisitor visitor =
                new ReferenceVisibilityVisitor(this);

            return visitor;
        }

        #endregion
    }
}
