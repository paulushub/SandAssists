using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Contents;

namespace Sandcastle
{
    [Serializable]
    public abstract class BuildGroup : BuildObject<BuildGroup>
    {
        #region Private Fields

        private string _groupName;
        private string _workingDir;

        private List<LinkContent>     _listLinks;
        private List<TokenContent>    _listTokens;
        private List<MediaContent>    _listMedia;
        private List<SharedContent>   _listShared;
        private List<SnippetContent>  _listSnippets;
        private List<ResourceContent> _listResources;

        #endregion

        #region Constructors and Destructor

        protected BuildGroup()
        {
            _listLinks     = new List<LinkContent>();
            _listMedia     = new List<MediaContent>();
            _listTokens    = new List<TokenContent>();
            _listShared    = new List<SharedContent>();
            _listSnippets  = new List<SnippetContent>();
            _listResources = new List<ResourceContent>();
        }

        protected BuildGroup(BuildGroup source)
            : base(source)
        {
            _listLinks     = source._listLinks;
            _listMedia     = source._listMedia;
            _listTokens    = source._listTokens;
            _listShared    = source._listShared;
            _listSnippets  = source._listSnippets;
            _listResources = source._listResources;
        }

        #endregion

        #region Public Properties

        public abstract bool IsEmpty
        {
            get;
        }

        public abstract BuildGroupType GroupType
        {
            get;
        }

        public string Name
        {
            get
            {
                return _groupName;
            }
            set
            {
                _groupName = value;
            }
        }

        public string WorkingDirectory
        {
            get
            {
                return _workingDir;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    value = Environment.ExpandEnvironmentVariables(value);
                    value = Path.GetFullPath(value);
                }
                _workingDir = value;
            }
        }

        public IList<LinkContent> LinkContents
        {
            get
            {
                return _listLinks;
            }
        }

        public IList<SharedContent> SharedContents
        {
            get
            {
                return _listShared;
            }
        }

        public IList<TokenContent> TokenContents
        {
            get
            {
                return _listTokens;
            }
        }

        public IList<MediaContent> MediaContents
        {
            get
            {
                return _listMedia;
            }
        }

        public IList<SnippetContent> SnippetContents
        {
            get
            {
                return _listSnippets;
            }
        }

        public IList<ResourceContent> ResourceContents
        {
            get
            {
                return _listResources;
            }
        }

        #endregion

        #region Public Methods

        #region Initialization Methods

        public virtual bool Initialize(BuildSettings settings)
        {
            BuildExceptions.NotNull(settings, "settings");

            return true;
        }

        public virtual void Uninitialize()
        {
        }

        #endregion

        #region Links Methods

        public virtual void AddLinkItem(string linkFile)
        {
            if (String.IsNullOrEmpty(linkFile))
            {
                return;
            }

            if (_listLinks == null)
            {
                _listLinks = new List<LinkContent>();
            }
            LinkContent defaultContent = null;
            if (_listLinks.Count == 0)
            {
                defaultContent = new LinkContent();
                _listLinks.Add(defaultContent);
            }
            else
            {
                defaultContent = _listLinks[0];
            }

            defaultContent.Add(new LinkItem(linkFile));
        }

        public virtual void AddLinkItem(string linkDir, bool isRecursive)
        {
            if (String.IsNullOrEmpty(linkDir))
            {
                return;
            }

            if (_listLinks == null)
            {
                _listLinks = new List<LinkContent>();
            }
            LinkContent defaultContent = null;
            if (_listLinks.Count == 0)
            {
                defaultContent = new LinkContent();
                _listLinks.Add(defaultContent);
            }
            else
            {
                defaultContent = _listLinks[0];
            }

            defaultContent.Add(new LinkItem(linkDir, isRecursive));
        }

        public virtual void AddLink(LinkContent content)
        {
            BuildExceptions.NotNull(content, "content");

            if (_listLinks == null)
            {
                _listLinks = new List<LinkContent>();
            }

            _listLinks.Add(content);
        }

        #endregion

        #region Resources Methods

        public virtual void AddResourceItem(string source, string destination)
        {
            if (String.IsNullOrEmpty(source) ||
                String.IsNullOrEmpty(destination))
            {
                return;
            }

            if (_listResources == null)
            {
                _listResources = new List<ResourceContent>();
            }
            ResourceContent defaultContent = null;
            if (_listResources.Count == 0)
            {
                defaultContent = new ResourceContent();
                _listResources.Add(defaultContent);
            }
            else
            {
                defaultContent = _listResources[0];
            }

            defaultContent.Add(new ResourceItem(source, destination));
        }

        public virtual void AddResource(ResourceContent content)
        {
            BuildExceptions.NotNull(content, "content");

            if (_listResources == null)
            {
                _listResources = new List<ResourceContent>();
            }

            _listResources.Add(content);
        }

        #endregion

        #region Media Methods

        public virtual void AddMedia(MediaContent content)
        {
            BuildExceptions.NotNull(content, "content");

            if (_listMedia == null)
            {
                _listMedia = new List<MediaContent>();
            }

            _listMedia.Add(content);
        }

        #endregion

        #region Tokens Methods

        public virtual void AddToken(TokenContent content)
        {
            BuildExceptions.NotNull(content, "content");

            if (_listTokens == null)
            {
                _listTokens = new List<TokenContent>();
            }

            _listTokens.Add(content);
        }

        #endregion

        #region Shared Methods

        public virtual void AddShared(SharedContent content)
        {
            BuildExceptions.NotNull(content, "content");

            if (_listShared == null)
            {
                _listShared = new List<SharedContent>();
            }

            _listShared.Add(content);
        }

        #endregion

        #region Snippets Methods

        public virtual void AddSnippet(SnippetContent content)
        {
            BuildExceptions.NotNull(content, "content");

            if (_listSnippets == null)
            {
                _listSnippets = new List<SnippetContent>();
            }

            _listSnippets.Add(content);
        }

        #endregion

        #endregion
    }
}
