using System;
using System.IO;
using System.Text;

namespace Sandcastle
{
    [Serializable]
    public abstract class BuildStep : BuildObject<BuildStep>
    {
        #region Private Fields

        private bool      _isEnabled;
        private bool      _continueOnError;
        private string    _name;
        private string    _description;
        private string    _workingDir;

        private BuildStep _preStep;
        private BuildStep _postStep;

        [NonSerialized] 
        private BuildContext _context;

        #endregion

        #region Constructors and Destructor

        protected BuildStep()
        {
            _isEnabled = true;
        }

        protected BuildStep(string workingDir)
        {
            _isEnabled  = true;
            _workingDir = workingDir;
        }

        protected BuildStep(BuildStep source)
            : base(source)
        {
            _workingDir       = source._workingDir;
        }

        #endregion

        #region Public Properties

        public bool Enabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
            }
        }
        
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
            }
        }

        public bool ContinueOnError
        {
            get
            {
                return _continueOnError;
            }
            set
            {
                _continueOnError = value;
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
                _workingDir = value;
            }
        }

        public virtual bool IsProcess
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsMultiSteps
        {
            get
            {
                return false;
            }
        }

        public virtual bool IsCancellable
        {
            get
            {
                return false;
            }
        }

        public BuildStep PreStep
        {
            get
            {
                return _preStep;
            }
            set
            {
                _preStep = value;
            }
        }

        public BuildStep PostStep
        {
            get
            {
                return _postStep;
            }
            set
            {
                _postStep = value;
            }
        }

        public BuildContext Context
        {
            get 
            { 
                return _context; 
            }
        }

        #endregion

        #region Public Methods

        public virtual bool Initialize(BuildContext context)
        {
            BuildExceptions.NotNull(context, "context");

            if (_preStep != null)
            {
                _preStep.Initialize(context);
            }
            if (_postStep != null)
            {
                _postStep.Initialize(context);
            }

            _context = context;

            return true;
        }

        public virtual bool Uninitialize(BuildContext context)
        {
            BuildExceptions.NotNull(context, "context");

            if (_context != null)
            {
                if (_context != context)
                {
                    throw new BuildException("The build context must match.");
                }
            }

            if (_preStep != null)
            {
                _preStep.Uninitialize(context);
            }
            if (_postStep != null)
            {
                _postStep.Uninitialize(context);
            }

            _context = null;

            return true;
        }

        public bool Execute()
        {
            if (_context == null)
            {
                throw new BuildException(
                    "There is no build context associated with this build step.");
            }

            BuildEngine engine = _context.Engine;
            if (engine == null)
            {
                throw new BuildException(
                    "There is no build engine associated with the the build context.");
            }
            if (this.PreExecute(engine) == false)
            {
                if (_preStep.ContinueOnError == false)
                {
                    return false;
                }
            }

            bool mainResult = this.MainExecute(engine);
            if (mainResult == false)
            {
                if (this.ContinueOnError == false)
                {
                    return false;
                }
            }

            if (this.PostExecute(engine) == false)
            {
                if (_postStep.ContinueOnError == false)
                {
                    return false;
                }
            }

            return mainResult;
        }

        public virtual bool Cancel()
        {
            return true;
        }

        #endregion

        #region Protected Methods

        protected abstract bool MainExecute(BuildEngine engine);

        protected virtual bool PreExecute(BuildEngine engine)
        {
            if (_preStep == null || _preStep.Enabled == false)
            {
                return true;
            }

            return _preStep.Execute();
        }

        protected virtual bool PostExecute(BuildEngine engine)
        {
            if (_postStep == null || _postStep.Enabled == false)
            {
                return true;
            }

            return _postStep.Execute();
        }

        protected string ExpandPath(string inputFile)
        {
            string outputFile = Environment.ExpandEnvironmentVariables(inputFile);
            if (!Path.IsPathRooted(outputFile))
            {
                if (!String.IsNullOrEmpty(_workingDir))
                {
                    outputFile = Path.Combine(_workingDir, outputFile);
                }
            }
            outputFile = Path.GetFullPath(outputFile);

            return outputFile;
        }

        #endregion
    }
}
