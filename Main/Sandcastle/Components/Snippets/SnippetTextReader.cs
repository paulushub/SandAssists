using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components.Snippets
{
    public sealed class SnippetTextReader : SnippetReader
    {
        #region Constructors and Destructor

        public SnippetTextReader(int tabSize, Type componentType,
            MessageHandler msgHandler)
            : base(tabSize, componentType, msgHandler)
        {
        }

        #endregion

        #region Public Methods

        public override void Read(string dataSource, SnippetProvider provider)
        {
            if (dataSource == null)
            {
                throw new ArgumentNullException("dataSource",
                    "The data source cannot be null (or Nothing).");
            }
            if (dataSource.Length == 0)
            {
                throw new ArgumentException(
                    "The data source cannot be empty.", "dataSource");
            }
            if (provider == null)
            {
                throw new ArgumentNullException("provider",
                    "The snippet provider cannot be null (or Nothing).");
            }

            //int tabSize = this.TabSize;
        }

        public override void Read(IList<string> dataSources,
            SnippetProvider provider)
        {
            if (dataSources == null)
            {
                throw new ArgumentNullException("dataSources",
                    "The data sources cannot be null (or Nothing).");
            }
            if (provider == null)
            {
                throw new ArgumentNullException("provider",
                    "The snippet provider cannot be null (or Nothing).");
            }

            int itemCount = dataSources.Count;
            for (int i = 0; i < itemCount; i++)
            {
                string dataSource = dataSources[i];
                if (String.IsNullOrEmpty(dataSource) == false)
                {
                    this.Read(dataSource, provider);
                }
            }
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
