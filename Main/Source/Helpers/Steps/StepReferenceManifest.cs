using System;
using System.IO;
using System.Diagnostics;

using Sandcastle.References;

namespace Sandcastle.Steps
{
    public sealed class StepReferenceManifest : StepXslTransform
    {
        #region Private Fields

        [NonSerialized]
        private ReferenceGroup _group;

        #endregion

        #region Constructors and Destructor

        public StepReferenceManifest()
        {
        }

        public StepReferenceManifest(string workingDir, string arguments)
            : base(workingDir, arguments)
        {
        }

        public StepReferenceManifest(string workingDir, string fileName, string arguments)
            : base(workingDir, fileName, arguments)
        {
        }

        #endregion

        #region Public Properties

        public ReferenceGroup Group
        {
            get
            {
                return _group;
            }
            set
            {
                _group = value;
            }
        }

        #endregion

        #region Protected Methods

        #region MainExecute Method

        protected override bool OnExecute(BuildContext context)
        {
            Debug.Assert(_group != null);
            if (_group == null)
            {
                throw new BuildException(
                    "A build group is required, but none is attached to this task.");
            }

            bool buildResult = base.OnExecute(context);

            if (!buildResult)
            {
                return false;
            }

            return buildResult;
        }

        #endregion

        #endregion

        #region Private Methods

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
