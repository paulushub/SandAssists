using System;
using System.Text;
using System.Collections.Generic;

namespace ICSharpCode.AddInManager
{
    /// <summary>
    /// This provides a toolbar button-style properties and methods, and it is used as
    /// a replacement for the button operations in the Add-In manager dialog.
    /// </summary>
    public sealed class AddInActionHelper
    {
        #region Private Fields

        private bool   _isEnabled;
        private bool   _isVisible;
        private bool   _isUndoable;
        private string _actionText;
        private string _undoText;
        private object _actionTag;

        #endregion

        #region Constructors and Destructor

        public AddInActionHelper()
        {
            _isEnabled = true;
            _isVisible = true;
        }

        public AddInActionHelper(string text)
            : this()
        {
            _actionText = text;
        }

        #endregion

        #region Public Events

        public event EventHandler Click;
        public event EventHandler UndoableChanged;
        public event EventHandler TextChanged;

        #endregion

        #region Public Properties

        public bool Enabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        public bool Visible
        {
            get { return _isVisible; }
            set { _isVisible = value; }
        }

        public bool Undoable
        {
            get { return _isUndoable; }
            set 
            { 
                _isUndoable = value; 
                if (this.UndoableChanged != null)
                {
                    this.UndoableChanged(this, EventArgs.Empty);
                }
            }
        }

        public string Text
        {
            get { return _actionText; }
            set 
            { 
                _actionText = value; 
                if (this.TextChanged != null)
                {
                    this.TextChanged(this, EventArgs.Empty);
                }
            }
        }

        public string UndoText
        {
            get { return _undoText; }
            set
            {
                _undoText = value;
                //if (this.TextChanged != null)
                //{
                //    this.TextChanged(this, EventArgs.Empty);
                //}
            }
        }

        public object Tag
        {
            get { return _actionTag; }
            set { _actionTag = value; }
        }

        #endregion

        #region Public Methods

        public void PerformClick()
        {
            if (this.Click != null)
            {
                this.Click(this, EventArgs.Empty);
            }
        }

        #endregion
    }
}
