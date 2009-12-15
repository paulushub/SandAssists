using System;
using System.IO;

namespace Sandcastle.Workshop.StartPage
{
    public sealed class StartPageItem : IEquatable<StartPageItem>
    {
        #region Private Fields

        private int      _solutionIndex;
        private string   _solutionPath;
        private string   _solutionName;
        private DateTime _solutionLastWrite;

        #endregion

        #region Constructors and Destructor

        public StartPageItem(int index, string solution)
        {   
            if (String.IsNullOrEmpty(solution))
            {   
                throw new ArgumentException("solution");
            }

            _solutionIndex     = index;
            _solutionPath      = Path.GetFullPath(solution);
            _solutionName      = Path.GetFileNameWithoutExtension(_solutionPath);
            _solutionLastWrite = DateTime.MinValue;

            if (File.Exists(_solutionPath))
            {
                _solutionLastWrite = File.GetLastWriteTime(_solutionPath);
            }
        }

        #endregion

        #region Public Properties

        public int Index
        {
            get
            {
                return _solutionIndex;
            }
        }

        public string Name
        {
            get
            {
                return _solutionName;
            }
        }

        public string FullPath
        {
            get
            {
                return _solutionPath;
            }
        }

        public string LastModified
        {
            get
            {
                if (_solutionLastWrite != DateTime.MinValue)
                {
                    return _solutionLastWrite.ToShortDateString();
                }

                return String.Empty;
            }
        }

        public DateTime LastModifiedDate
        {
            get
            {
                return _solutionLastWrite;
            }
        }

        public string[] CreateCells()
        {
            return new string[] { this.Index.ToString(), this.Name, this.LastModified, this.FullPath };
        }

        #endregion

        #region IEquatable<StartPageItem> Members

        public bool Equals(StartPageItem other)
        {
            if (other == null)
            {
                return false;
            }

            return String.Equals(this._solutionPath, 
                other._solutionPath, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as StartPageItem);
        }

        public override int GetHashCode()
        {
            if (_solutionPath != null)
            {
                return _solutionPath.ToLowerInvariant().GetHashCode();
            }

            return base.GetHashCode();
        }

        #endregion
    }
}
