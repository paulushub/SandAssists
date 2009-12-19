using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Bookmarks;

namespace ICSharpCode.XmlEditor
{
    /// <summary>
    /// This is a view content that is based on another (primary) view content,
    /// and supports XML file formats.
    /// </summary>
    /// <remarks>
    /// This is similar to the <see cref="AbstractSecondaryViewContent"/> class,
    /// but is is based on <see cref="XmlView"/> and it is not <see langword="abstract"/>.
    /// </remarks>
    public class XmlSecondaryView : XmlView
    {
        #region Private Fields

        private IViewContent primaryViewContent;
        private OpenedFile primaryFile;

        #endregion

        #region Constructors and Destructor

        public XmlSecondaryView(IViewContent primaryViewContent)
        {
            if (primaryViewContent == null)
                throw new ArgumentNullException("primaryViewContent");
            if (primaryViewContent.PrimaryFile == null)
                throw new ArgumentException("primaryViewContent.PrimaryFile must not be null");
            this.primaryViewContent = primaryViewContent;

            primaryFile = primaryViewContent.PrimaryFile;
            this.Files.Add(primaryFile);
        }

        #endregion

        #region Public Properties

        public IViewContent PrimaryViewContent
        {
            get { return primaryViewContent; }
        }

        public sealed override OpenedFile PrimaryFile
        {
            get { return primaryFile; }
        }

        /// <summary>
        /// Gets the list of sibling secondary view contents.
        /// </summary>
        public override ICollection<IViewContent> SecondaryViewContents
        {
            get { return primaryViewContent.SecondaryViewContents; }
        }

        #endregion

        #region Public Methods

        public override void Load(OpenedFile file, Stream stream)
        {
            if (file != this.PrimaryFile)
                throw new ArgumentException("file must be the primary file of the primary view content, override Load() to handle other files");
            primaryViewContent.Load(file, stream);
            LoadFromPrimary();
        }

        public override void Save(OpenedFile file, Stream stream)
        {
            if (file != this.PrimaryFile)
                throw new ArgumentException("file must be the primary file of the primary view content, override Save() to handle other files");
            SaveToPrimary();
            primaryViewContent.Save(file, stream);
        }

        public override bool SupportsSwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView)
        {
            if (file == this.PrimaryFile)
            {
                if (primaryViewContent == newView)
                {
                    return newView.SupportsSwitchToThisWithoutSaveLoad(file, this);
                }

                return newView.SupportsSwitchToThisWithoutSaveLoad(file, primaryViewContent);
            }
            else
            {
                return base.SupportsSwitchFromThisWithoutSaveLoad(file, newView);
            }
        }

        public override bool SupportsSwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView)
        {
            if (file == this.PrimaryFile)
            {
                if (primaryViewContent == oldView)
                {
                    return oldView.SupportsSwitchToThisWithoutSaveLoad(file, this);
                }
                return oldView.SupportsSwitchToThisWithoutSaveLoad(file, primaryViewContent);
            }
            else
            {
                return base.SupportsSwitchFromThisWithoutSaveLoad(file, oldView);
            }
        }

        public override void SwitchFromThisWithoutSaveLoad(OpenedFile file, IViewContent newView)
        {
            if (file == this.PrimaryFile && this != newView)
            {
                SaveToPrimary();
                primaryViewContent.SwitchFromThisWithoutSaveLoad(file, newView);
            }
        }

        public override void SwitchToThisWithoutSaveLoad(OpenedFile file, IViewContent oldView)
        {
            if (file == this.PrimaryFile && oldView != this)
            {
                primaryViewContent.SwitchToThisWithoutSaveLoad(file, oldView);
                LoadFromPrimary();
            }
        }

        #endregion

        #region Protected Methods

        protected virtual void LoadFromPrimary()
        {
            XmlEditorControl xmlEditor = this.XmlEditor;
            if (xmlEditor == null || primaryFile == null)
            {
                return;
            }

            if (!primaryFile.IsUntitled)
            {
                xmlEditor.IsReadOnly = IsFileReadOnly(primaryFile.FileName);
            }

            xmlEditor.LoadFile(primaryFile.FileName, false, true);
            foreach (BookmarkEx mark in BookmarkExManager.GetBookmarks(primaryFile.FileName))
            {
                mark.Document = xmlEditor.Document;
                xmlEditor.Document.BookmarkManager.AddMark(mark);
            }
            UpdateFolding();
        }

        protected virtual void SaveToPrimary()
        {
            XmlEditorControl xmlEditor = this.XmlEditor;
            if (xmlEditor == null || primaryFile == null)
            {
                return;
            }

            xmlEditor.SaveFile(primaryFile.FileName);
        }

        #endregion
    }
}
