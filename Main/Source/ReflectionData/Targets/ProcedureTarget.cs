// Copyright ｩ Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Serialization;

using Sandcastle.ReflectionData.References;

namespace Sandcastle.ReflectionData.Targets
{
    [Serializable]
    public class ProcedureTarget : MemberTarget
    {
        #region Private Fields

        internal bool conversionOperator;
        internal MemberReference explicitlyImplements;

        #endregion

        #region Constructors and Destructor

        public ProcedureTarget()
        {
        }

        #endregion

        #region Public Properties

        public override TargetType TargetType
        {
            get
            {
                return TargetType.Procedure;
            }
        }

        public bool ConversionOperator
        {
            get
            {
                return conversionOperator;
            }
        }

        public MemberReference ExplicitlyImplements
        {
            get
            {
                return explicitlyImplements;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        protected override void OnReadXml(XmlReader reader)
        {
            base.OnReadXml(reader);

            // This reads only the target node...
            if (!String.Equals(reader.Name, "ProcedureTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string tempText = reader.GetAttribute("conversionOperator");
            if (!String.IsNullOrEmpty(tempText))
            {
                conversionOperator = Boolean.Parse(tempText);
            }

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    // Read the base contents in...
                    if (String.Equals(reader.Name, "MemberTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        base.OnReadXml(reader);
                    }
                    else if (reader.Name.IndexOf("Member", 0, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        explicitlyImplements = ReferencesReader.ReadMemberReference(reader);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "ProcedureTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        protected override void OnWriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("ProcedureTarget");
            writer.WriteAttributeString("conversionOperator", conversionOperator.ToString());

            // Write the base contents in...
            base.OnWriteXml(writer);

            if (explicitlyImplements != null)
            {
                explicitlyImplements.WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "ProcedureTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                this.OnReadXml(reader);
            }
            else
            {
                XmlNodeType nodeType = XmlNodeType.None;
                while (reader.Read())
                {
                    nodeType = reader.NodeType;

                    if (nodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(reader.Name, "ProcedureTarget",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            this.OnReadXml(reader);

                            if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "ProcedureTarget",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "ProcedureTarget",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }
                    }
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            this.OnWriteXml(writer);
        }

        #endregion
    }
}
