using System;

namespace Sandcastle.Utilities
{
    [Serializable]
    public class ProcessEventArgs : EventArgs
    {
        #region Private Fields

        private string _outputText;

        #endregion

        #region Constructors and Destructor

        public ProcessEventArgs()
        {
        }

        public ProcessEventArgs(string output)
        {
            _outputText = output;
        }

        #endregion

        #region Public Properties

        public string Output
        {
            get
            {
                return _outputText;
            }
        }

        #endregion
    }
}
