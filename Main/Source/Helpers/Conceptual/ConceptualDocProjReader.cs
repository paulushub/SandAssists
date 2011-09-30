using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Conceptual
{
    public sealed class ConceptualDocProjReader : ConceptualReader
    {
        #region Private Fields

        private string _contentDir;

        #endregion

        #region Constructors and Destructor

        public ConceptualDocProjReader()
        {
        }

        #endregion

        #region Public Methods

        public override ConceptualContent Read(string contentFile)
        {
            BuildExceptions.PathMustExist(contentFile, "contentFile");

            _contentDir = Path.GetDirectoryName(contentFile);

            return null;
        }

        #endregion
    }
}
