//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Reflection;

using Sandcastle.HelpRegister.Properties;

namespace Sandcastle.HelpRegister
{
	public class XmlValidator : MarshalByRefObject
    {
        #region Public Constant Fields

        public const string Help2NamespaceUri = "http://www.simmack.de/2006/help2";

        #endregion

        #region Private Fields

        private bool _validationSuccess = true;
        private bool _beQuiet;
        private bool _schemaExists;
        private string _xmlSchema;
        private Stream _xmlStream;
        private IRegistrationHelper _regHelper;

        #endregion

        #region Constructors and Destructor

        public XmlValidator(IRegistrationHelper helper)
        {
            if (helper == null)
            {
                throw new ArgumentNullException("helper",
                    "The helper parameter cannot be null (or Nothing).");
            }

            _regHelper = helper;
            _xmlSchema = Path.Combine(Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location), "register.xsd");

            if (File.Exists(_xmlSchema) == false)
            {
                _xmlSchema   = null;
                Assembly currentAssembly = Assembly.GetExecutingAssembly();
                try
                {
                    _xmlStream = currentAssembly.GetManifestResourceStream(
                        "Sandcastle.HelpRegister.Properties.Register.xsd");

                    _schemaExists = (_xmlStream != null && _xmlStream.Length != 0);
                }
                catch
                {
                }
            }
            else
            {
                _schemaExists = true;
            }
        }

        #endregion

        #region Public Properties

        public bool SchemaExists
		{
			get 
            {
                return _schemaExists; 
            }
        }

        #endregion

        #region Public Methods

        public bool Validate(string xmlCommandFile, bool silentMode)
		{
            if (_schemaExists == false)
            {
                return false;
            }

			_beQuiet = silentMode;

			XmlReaderSettings xsd = new XmlReaderSettings();
            if (_xmlStream != null && _xmlStream.Length != 0)
            {
                XmlTextReader textReader = new XmlTextReader(_xmlStream);
                xsd.Schemas.Add(XmlValidator.Help2NamespaceUri, textReader);
            }
            else
            {
                xsd.Schemas.Add(XmlValidator.Help2NamespaceUri, _xmlSchema);
            }
            xsd.CloseInput = true;
			xsd.ValidationType          = ValidationType.Schema;
			xsd.ValidationEventHandler += new ValidationEventHandler(ValidationCallback);

            using (XmlReader reader = XmlReader.Create(xmlCommandFile, xsd))
            {
                while (reader.Read())
                {
                }
            }
            if (_xmlStream != null)
            {
                _xmlStream.Close();
                _xmlStream = null;
            }

			return _validationSuccess;
        }

        #endregion

        #region Private Methods

        private void ValidationCallback(object sender, ValidationEventArgs e)
		{
            if (!_beQuiet)
            {
                if (_regHelper != null)
                {
                    _regHelper.WriteLine(e.Message);
                }
            }

			_validationSuccess = false;
        }

        #endregion
    }
}
