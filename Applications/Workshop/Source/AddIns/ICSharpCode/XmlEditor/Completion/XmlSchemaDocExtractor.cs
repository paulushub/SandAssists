// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 5323 $</version>
// </file>

using System;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace ICSharpCode.XmlEditor
{
	public sealed class XmlSchemaDocExtractor
    {
        #region Private Fields

        private StringBuilder _rawDocumentation;
        private StringBuilder _finalDocumentation;

        #endregion

        #region Constructors and Destructor

        public XmlSchemaDocExtractor()
		{
            _rawDocumentation   = new StringBuilder();
            _finalDocumentation = new StringBuilder();
        }

        #endregion

        #region Public Methods

        public string Extract(XmlSchemaAnnotation annotation)
        {
            _rawDocumentation.Length   = 0;
            _finalDocumentation.Length = 0;

            if (annotation != null)
            {
                ReadDocumentationFromAnnotation(annotation.Items);

                return _finalDocumentation.ToString();
            }

            return String.Empty;
        }

        #endregion

        #region Private Methods

        private void ReadDocumentationFromAnnotation(
            XmlSchemaObjectCollection annotationItems)
		{
			foreach (XmlSchemaObject schemaObject in annotationItems) {
				XmlSchemaDocumentation schemaDocumentation = schemaObject as XmlSchemaDocumentation;
				if (schemaDocumentation != null) {
					ReadSchemaDocumentationFromMarkup(schemaDocumentation.Markup);
				}
			}
			RemoveWhitespaceFromDocumentation();
		}

        private void ReadSchemaDocumentationFromMarkup(XmlNode[] markup)
		{
			foreach (XmlNode node in markup) {
				XmlText textNode = node as XmlText;
				AppendTextToDocumentation(textNode);
			}
		}

        private void AppendTextToDocumentation(XmlText textNode)
		{
			if (textNode != null) {
				if (textNode.Data != null) {
					_rawDocumentation.Append(textNode.Data);
				}
			}
		}

        private void RemoveWhitespaceFromDocumentation()
		{
            string finalDoc = _rawDocumentation.ToString();
            finalDoc = finalDoc.Trim();
            RemoveWhitespaceFromLines(finalDoc.Split('\n'));
		}

        private void RemoveWhitespaceFromLines(string[] lines)
		{
			foreach (string line in lines) {
				string lineWithoutWhitespace = line.Trim();
				_finalDocumentation.AppendLine(lineWithoutWhitespace);
			}
        }

        #endregion
    }
}
