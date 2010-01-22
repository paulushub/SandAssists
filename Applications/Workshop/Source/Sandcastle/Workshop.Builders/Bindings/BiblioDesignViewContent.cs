﻿using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace Sandcastle.Workshop.Bindings
{
    public sealed class BiblioDesignViewContent : ContentsDesignViewContent
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public BiblioDesignViewContent(OpenedFile file)
            : base(file)
        {
        }

        #endregion

        #region Public Properties

        public override string FileExtension
        {
            get
            {
                return ".biblio";
            }
        }

        #endregion
    }
}