using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections.Generic;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components.Snippets
{
    public sealed class SnippetDbProvider : SnippetProvider
    {
        #region Private Fields

        private Dictionary<SnippetInfo, IList<SnippetItem>> _dicSnippets;

        #endregion

        #region Constructors and Destructor

        public SnippetDbProvider(Type componentType,
            MessageHandler messageHandler)
            : base(componentType, messageHandler)
        {
            _dicSnippets = new Dictionary<SnippetInfo, IList<SnippetItem>>();
        }

        #endregion

        #region Public Properties

        public override int Count
        {
            get
            {
                if (_dicSnippets != null)
                {
                    return _dicSnippets.Count;
                }

                return 0;
            }
        }

        public override bool IsMemory
        {
            get
            {
                return false;
            }
        }

        public override SnippetStorage Storage
        {
            get
            {
                return SnippetStorage.Database;
            }
        }

        public override IList<SnippetItem> this[SnippetInfo info]
        {
            get
            {
                IList<SnippetItem> listSnippets;
                if (_dicSnippets.TryGetValue(info, out listSnippets))
                {
                    return listSnippets;
                }

                return null;
            }
        }

        #endregion

        #region Public Methods

        public override bool StartRegister(bool clearExisting)
        {
            return true;
        }

        public override void FinishRegister()
        {
        }

        public override void Register(Snippet snippet)
        {
            if (snippet == null)
            {
                throw new ArgumentNullException("snippet",
                    "The snippet cannot be null (or Nothing).");
            }

            this.Register(new SnippetInfo(snippet.ExampleId, snippet.SnippetId),
                new SnippetItem(snippet.Language, snippet.Text));
        }

        public override void Register(SnippetInfo info, SnippetItem item)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info",
                    "The snippet information cannot be null (or Nothing).");
            }
            if (item == null)
            {
                throw new ArgumentNullException("item",
                    "The snippet item cannot be null (or Nothing).");
            }
            if (_dicSnippets == null)
            {
                _dicSnippets = new Dictionary<SnippetInfo, IList<SnippetItem>>();
            }

            IList<SnippetItem> listSnippets;
            if (_dicSnippets.TryGetValue(info, out listSnippets) == false)
            {
                listSnippets = new List<SnippetItem>();
                _dicSnippets.Add(info, listSnippets);
            }
            listSnippets.Add(item);
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            _dicSnippets = null;
            base.Dispose(disposing);
        }

        #endregion
    }
}
