using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle
{
    /// <summary>
    /// This is an <see langword="abstract"/> base class defining a build step or task, 
    /// which is a unit of action or execution in the documentation process.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In a build process, all the valid build steps will run except when the
    /// <see cref="BuildStep.Enabled"/> property is set to <see langword="false"/>.
    /// </para>
    /// </remarks>
    public abstract class BuildStep : BuildObject, IDisposable
    {
        #region Private Fields

        private bool      _isEnabled;
        private bool      _logTimeSpan;
        private bool      _isInitialized;
        private bool      _continueOnError;
        private string    _name;
        private string    _title;
        private string    _message;
        private string    _description;
        private string    _workingDir;

        private Stopwatch _stepTimer;

        private BuildMultiStep _beforeSteps;
        private BuildMultiStep _replaceSteps;
        private BuildMultiStep _afterSteps;
        private BuildLoggerVerbosity _verbosity;
        private BuildProperties _properties;

        [NonSerialized] 
        private BuildContext _context;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildStep"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildStep"/> class
        /// to the default properties or values.
        /// </summary>
        protected BuildStep()
        {
            _isEnabled   = true;
            _logTimeSpan = true;
            _properties  = new BuildProperties();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildStep"/> class with the
        /// specified working directory.
        /// </summary>
        /// <param name="workingDir">
        /// A <see cref="System.String"/> containing the working directory.
        /// </param>
        protected BuildStep(string workingDir)
            : this()
        {
            _workingDir = workingDir;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildStep"/> class with the
        /// specified name and working directory.
        /// </summary>
        /// <param name="name">
        /// A <see cref="System.String"/> containing the name of this build step.
        /// </param>
        /// <param name="workingDir">
        /// A <see cref="System.String"/> containing the working directory of this
        /// build step.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="name"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="name"/> is empty.
        /// </exception>
        protected BuildStep(string name, string workingDir)
            : this(workingDir)
        {           
            BuildExceptions.NotNullNotEmpty(name, "name");

            _name = name;
        }

        /// <summary>
        /// This allows the <see cref="BuildStep"/> instance to attempt to free 
        /// resources and perform other cleanup operations before the 
        /// <see cref="BuildStep"/> instance is reclaimed by garbage collection.
        /// </summary>
        ~BuildStep()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value specifying the type of this build style.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="BuildStepType"/> specifying the type
        /// of this build step.
        /// </value>
        public virtual BuildStepType StepType
        {
            get
            {
                return BuildStepType.None;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this build step is enabled.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if this build step is allowed to
        /// run as part of the build process; otherwise, it is <see langword="false"/>.
        /// <para>
        /// The default is <see langword="true"/>.
        /// </para>
        /// </value>
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

        /// <summary>
        /// Gets or sets a value specifying the name of this build step.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the name of this build step.
        /// <para>
        /// The default value is <see cref="String.Empty">String.Empty</see>.
        /// </para>
        /// </value>
        /// <remarks>
        /// The following must be noted in the value of this property:
        /// <list type="number">
        /// <item>
        /// <description>
        /// The value can be set only once or once the value of this property is set
        /// it cannot be altered in a build process.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// The <see langword="null"/> and <see cref="String.Empty">String.Empty</see> 
        /// are not allowed and are just ignored.
        /// </description>
        /// </item>
        /// </list>
        /// </remarks>
        public string Name
        {
            get
            {
                return _name;
            }
            protected set
            {       
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }   
                if (String.IsNullOrEmpty(_name))
                {
                    _name = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the title displayed by this build step.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the message displayed by this
        /// build step.
        /// </value>
        public string LogTitle
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
            }
        }

        /// <summary>
        /// Gets or sets the message displayed by this build step.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the message displayed by this
        /// build step.
        /// </value>
        /// <remarks>
        /// The message is displayed at the beginning of the build process.
        /// </remarks>
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
            }
        }

        /// <summary>
        /// Gets or sets the description of this build step. This is not used by the
        /// build process or framework.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the description of this build step.
        /// </value>
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

        /// <summary>
        /// Gets or sets a value indicating whether to continue the build process
        /// when there is an error in this build step.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if the build process will continue
        /// when this build step fails; otherwise, it is <see langword="false"/>.
        /// <para>
        /// The default value is <see langword="false"/>.
        /// </para>
        /// </value>
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

        /// <summary>
        /// Gets a value indicating whether this build step is initialized and ready 
        /// for the build process.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if this build step is initialized;
        /// otherwise, it is <see langword="false"/>.
        /// </value>
        /// <seealso cref="BuildStep.Initialize(BuildContext)"/>
        /// <seealso cref="BuildStep.Uninitialize()"/>
        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
            protected set
            {
                _isInitialized = value;
            }
        }

        /// <summary>
        /// Gets or sets the working directory for this build step.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the build directory of this
        /// build step.
        /// </value>
        /// <remarks>
        /// In most cases, this is the same as the working directory of the build
        /// process, but it is not guarantee to be the same.
        /// </remarks>
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

        /// <summary>
        /// Gets a value indicating whether this step is a local system process task.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if this step executes a local
        /// system process; otherwise, it is <see langword="false"/>.
        /// <para>
        /// The default value, returned by this base class, is <see langword="false"/>.
        /// </para>
        /// </value>
        public virtual bool IsProcess
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this build step is a multi-step task.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if this build step is multi-step;
        /// otherwise, it is <see langword="false"/>.
        /// <para>
        /// The default value, returned by this base class, is <see langword="false"/>.
        /// </para>
        /// </value>
        /// <seealso cref="BuildMultiStep"/>
        public virtual bool IsMultiSteps
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this build step can be cancelled.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if this build step can be 
        /// cancelled; otherwise, it is <see langword="false"/>.
        /// <para>
        /// The default value, returned by this base class, is <see langword="false"/>.
        /// </para>
        /// </value>
        /// <remarks>
        /// For a build step that can be cancelled, the build process can call the
        /// <see cref="BuildStep.Cancel()"/> method to cancel the task.
        /// <para>
        /// For build steps that cannot be cancelled, the steps are expected to
        /// monitor the state of the <see cref="BuildContext.IsCancelled"/> property
        /// and response automatically (canceling the task themselves), where possible.
        /// </para>
        /// </remarks>
        /// <seealso cref="BuildStep.Cancel()"/>
        public virtual bool IsCancellable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the pre-build steps for this build step, the pre-build steps are 
        /// the build tasks that will be executed before this build step.
        /// </summary>
        /// <value>
        /// A list of the <see cref="BuildStep"/> class specifying the tasks that
        /// must be executed before the task defined by this build step.
        /// </value>
        /// <remarks>
        /// The pre-build steps are defined using the multi-step, 
        /// <see cref="BuildMultiStep"/>, allowing the execution of several steps.
        /// </remarks>
        /// <seealso cref="BuildStep.ReplaceSteps"/>
        /// <seealso cref="BuildStep.AfterSteps"/>
        public IList<BuildStep> BeforeSteps
        {
            get
            {
                if (_beforeSteps == null)
                {
                    return null;
                }

                return _beforeSteps.Steps;
            }
        }

        /// <summary>
        /// Gets the replacement steps for this build step, the replacement steps
        /// defined the build tasks that will be executed instead this build step.
        /// </summary>
        /// <value>
        /// A list of the <see cref="BuildStep"/> class specifying the tasks that
        /// must be executed instead the task defined by this build step.
        /// </value>
        /// <remarks>
        /// The replacement steps are defined using the multi-step, 
        /// <see cref="BuildMultiStep"/>, allowing the execution of several steps.
        /// </remarks>
        /// <seealso cref="BuildStep.BeforeSteps"/>
        /// <seealso cref="BuildStep.AfterSteps"/>
        public IList<BuildStep> ReplaceSteps
        {
            get
            {
                if (_replaceSteps == null)
                {
                    return null;
                }

                return _replaceSteps.Steps;
            }
        }

        /// <summary>
        /// Gets the post-build steps for this build step, the post-build steps 
        /// defined the build tasks that will be executed after this build step.
        /// </summary>
        /// <value>
        /// A list of the <see cref="BuildStep"/> class specifying the tasks that
        /// must be executed after the task defined by this build step.
        /// </value>
        /// <remarks>
        /// The post-build steps are defined using the multi-step, 
        /// <see cref="BuildMultiStep"/>, allowing the execution of several steps.
        /// </remarks>
        /// <seealso cref="BuildStep.BeforeSteps"/>
        /// <seealso cref="BuildStep.ReplaceSteps"/>
        public IList<BuildStep> AfterSteps
        {
            get
            {
                if (_afterSteps == null)
                {
                    return null;
                }

                return _afterSteps.Steps;
            }
        }

        /// <summary>
        /// Gets the current context of the build process.
        /// </summary>
        /// <value>
        /// An instance of the <see cref="BuildContext"/> class, if this build step is
        /// initialized; otherwise, it is <see langword="null"/>.
        /// </value>
        /// <seealso cref="BuildStep.Initialize(BuildContext)"/>
        public BuildContext Context
        {
            get 
            { 
                return _context; 
            }
        }

        /// <summary>
        /// Gets the total elapsed time measured in the execution of this build step.
        /// </summary>
        /// <value>
        /// A <see cref="System.TimeSpan"/> representing the total elapsed time 
        /// measured in this build step. If this step is not executed, the value of
        /// this property will be <see cref="System.TimeSpan.Zero"/>.
        /// </value>
        public TimeSpan TimeElapsed
        {
            get
            {
                if (_stepTimer != null)
                {
                    return _stepTimer.Elapsed;
                }

                return TimeSpan.Zero;
            }
        }

        /// <summary>
        /// Gets or sets the level of detail to show in the build log.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="BuildLoggerVerbosity"/> specifying 
        /// the level of detail. The default is <see cref="BuildLoggerVerbosity.None"/>,
        /// in which case it is set in the initialization.
        /// </value>
        public BuildLoggerVerbosity Verbosity
        {
            get 
            { 
                return _verbosity; 
            }
            set 
            { 
                _verbosity = value; 
            }
        }

        /// <summary>
        /// Gets or sets the string value associated with the specified string key.
        /// </summary>
        /// <param name="key">The string key of the value to get or set.</param>
        /// <value>
        /// The string value associated with the specified string key. If the 
        /// specified key is not found, a get operation returns 
        /// <see langword="null"/>, and a set operation creates a new element 
        /// with the specified key.
        /// </value>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="key"/> is empty.
        /// </exception>
        public string this[string key]
        {
            get
            {
                return _properties[key];
            }
            set
            {
                _properties[key] = value;
            }
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="BuildSettings"/>.
        /// </summary>
        /// <value>
        /// The number of key/value pairs contained in the <see cref="BuildSettings"/>.
        /// </value>
        public int PropertyCount
        {
            get
            {
                return _properties.Count;
            }
        }

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="BuildSettings"/>.
        /// </summary>
        /// <value>
        /// A collection containing the keys in the <see cref="BuildSettings"/>.
        /// </value>
        public ICollection<string> PropertyKeys
        {
            get
            {
                return _properties.Keys;
            }
        }

        /// <summary>
        /// Gets a collection containing the values in the <see cref="BuildSettings"/>.
        /// </summary>
        /// <value>
        /// A collection containing the values in the <see cref="BuildSettings"/>.
        /// </value>
        public ICollection<string> PropertyValues
        {
            get
            {
                return _properties.Values;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the total time taken by this
        /// build step is written to the logger.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if the time span is logged; otherwise,
        /// it is <see langword="false"/>. The default is <see langword="true"/>,
        /// for most build steps, except the multi-step and message step.
        /// </value>
        public bool LogTimeSpan
        {
            get 
            { 
                return _logTimeSpan; 
            }
            set 
            { 
                _logTimeSpan = value; 
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes this build step with the specified current build context
        /// of the build process. This prepares the build step for the task execution.
        /// </summary>
        /// <param name="context">
        /// An instance of the <see cref="BuildContext"/> class, specifying the current
        /// build context of the build process.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="context"/> is <see langword="null"/>.
        /// </exception>
        /// <seealso cref="BuildStep.IsInitialized"/>
        /// <seealso cref="BuildStep.Uninitialize()"/>
        public virtual void Initialize(BuildContext context)
        {                        
            BuildExceptions.NotNull(context, "context");

            _isInitialized = false;
            if (_beforeSteps != null && _beforeSteps.Enabled)
            {
                _beforeSteps.Initialize(context);

                if (!_beforeSteps.IsInitialized)
                {
                    return;
                }
            }

            if (_afterSteps != null && _afterSteps.Enabled)
            {
                _afterSteps.Initialize(context);

                if (!_afterSteps.IsInitialized)
                {
                    return;
                }
            }

            _context = context;
            if (_verbosity == BuildLoggerVerbosity.None)
            {
                _verbosity = _context.Settings.Logging.Verbosity;
            }

            _isInitialized = true;
        }

        /// <summary>
        /// This provides un-initialization of the build step, and release any 
        /// resources created in the initialization process.
        /// </summary>
        /// <remarks>
        /// It provides the build step the way to clean up resources allocated
        /// during the initialization and execution of the build step.
        /// </remarks>
        /// <seealso cref="BuildStep.IsInitialized"/>
        /// <seealso cref="BuildStep.Initialize(BuildContext)"/>
        public virtual void Uninitialize()
        {
            if (_beforeSteps != null && _beforeSteps.Enabled)
            {
                _beforeSteps.Uninitialize();
            }
            if (_afterSteps != null && _afterSteps.Enabled)
            {
                _afterSteps.Uninitialize();
            }

            _context       = null;
            _verbosity     = BuildLoggerVerbosity.None;
            _isInitialized = false;
        }

        /// <summary>
        /// This executes or runs the task defined by this build step.
        /// </summary>
        /// <returns>
        /// This returns <see langword="true"/> if the task is successfully; otherwise,
        /// it is returns <see langword="false"/>.
        /// </returns>
        public bool Execute()
        {
            if (!_isInitialized)
            {
                throw new BuildException("The build step is not initialized");
            }
            if (_context == null)
            {
                throw new BuildException(
                    "There is no build context associated with this build step.");
            }

            if (!this.OnBeforeExecute(_context))
            {
                if (!_beforeSteps.ContinueOnError)
                {
                    return false;
                }
            }

            bool mainResult = false;

            // We will replace this current step, if we have a valid replacement...
            if (_replaceSteps != null && _replaceSteps.Count != 0 &&
                _replaceSteps.Enabled)
            {
                mainResult = this.OnReplaceExecute(_context);
            }
            else
            {
                mainResult = this.OnDefaultExecute(_context);
            }

            if (!mainResult)
            {
                if (!this.ContinueOnError)
                {
                    return false;
                }

                mainResult = true;
            }

            if (!this.OnAfterExecute(_context))
            {
                if (!_afterSteps.ContinueOnError)
                {
                    return false;
                }

                mainResult = true;
            }

            return mainResult;
        }

        /// <summary>
        /// This signals the build step to cancel or stop the build task if the task
        /// is cancellable.
        /// </summary>
        /// <returns>
        /// This returns <see langword="true"/> if the operation is cancellable and it
        /// is successfully cancelled or stopped; otherwise, it returns 
        /// <see langword="false"/>.
        /// </returns>
        public bool Cancel()
        {
            return false;
        }

        public void Insert(BuildStep insertStep, BuildInsertType insertType)
        {
            BuildExceptions.NotNull(insertStep, "insertStep");

            switch (insertType)
            {
                case BuildInsertType.None:
                    break;
                case BuildInsertType.Before:
                    if (_beforeSteps == null)
                    {
                        _beforeSteps = new BuildMultiStep();
                    }
                    break;
                case BuildInsertType.Replace:
                    if (_replaceSteps == null)
                    {
                        _replaceSteps = new BuildMultiStep();
                    }
                    break;
                case BuildInsertType.After:
                    if (_afterSteps == null)
                    {
                        _afterSteps = new BuildMultiStep();
                    }
                    break;
            }

            if (insertType != BuildInsertType.None)
            {
                this.OnInsert(insertStep, insertType);
            }
        }

        #region Public Properties Methods

        /// <summary>
        /// This removes the element with the specified key from the <see cref="BuildSettings"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="key"/> is empty.
        /// </exception>
        public void RemoveProperty(string key)
        {
            _properties.Remove(key);
        }

        /// <summary>
        /// This removes all keys and values from the <see cref="BuildSettings"/>.
        /// </summary>
        public void ClearProperties()
        {
            _properties.Clear();
        }

        /// <summary>
        /// This adds the specified string key and string value to the <see cref="BuildSettings"/>.
        /// </summary>
        /// <param name="key">The string key of the element to add.</param>
        /// <param name="value">
        /// The value of the element to add. The value can be a <see langword="null"/>.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="key"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="key"/> is empty.
        /// <para>-or-</para>
        /// An element with the same key already exists in the <see cref="BuildSettings"/>.
        /// </exception>
        /// <remarks>
        /// You can also use the <see cref="BuildSettings.this[string]"/> property to add 
        /// new elements by setting the value of a key that does not exist in the 
        /// <see cref="BuildSettings"/>. However, if the specified key already 
        /// exists in the <see cref="BuildSettings"/>, setting the 
        /// <see cref="BuildSettings.this[string]"/> property overwrites the old value. 
        /// In contrast, the <see cref="BuildSettings.Add"/> method throws an 
        /// exception if a value with the specified key already exists.
        /// </remarks>
        public void AddProperty(string key, string value)
        {
            _properties.Add(key, value);
        }

        /// <summary>
        /// This determines whether the <see cref="BuildSettings"/> contains 
        /// the specified key.
        /// </summary>
        /// <param name="key">
        /// The key to locate in the <see cref="BuildSettings"/>.
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the <see cref="BuildSettings"/> 
        /// contains an element with the specified key; otherwise, <see langword="false"/>.
        /// </returns>
        public bool ContainsPropertyKey(string key)
        {
            return _properties.ContainsKey(key);
        }

        /// <summary>
        /// This determines whether the <see cref="BuildSettings"/> contains a specific value.
        /// </summary>
        /// <param name="value">
        /// The value to locate in the <see cref="BuildSettings"/>. The value can 
        /// be a <see langword="null"/>.
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the <see cref="BuildSettings"/> 
        /// contains an element with the specified value; otherwise, 
        /// <see langword="false"/>.
        /// </returns>
        public bool ContainsPropertyValue(string value)
        {
            return _properties.ContainsValue(value);
        }

        #endregion

        #endregion

        #region Protected Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected abstract bool OnExecute(BuildContext context);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual bool OnDefaultExecute(BuildContext context)
        {
            bool mainResult    = false;
            BuildLogger logger = context.Logger;
            try
            {
                this.OnStartTiming();

                if (logger != null)
                {
                    if (!String.IsNullOrEmpty(_title))
                    {
                        logger.WriteLine(_title, BuildLoggerLevel.Started);
                    }
                    if (!String.IsNullOrEmpty(_message))
                    {
                        logger.WriteLine(_message, BuildLoggerLevel.Info);
                    }
                }

                mainResult = this.OnExecute(_context);

                this.OnStopTiming();

                if (mainResult)
                {
                    if (logger != null && _logTimeSpan)
                    {
                        string timeInfo = String.Empty;
                        if (this.IsMultiSteps)
                        {
                            logger.WriteLine();
                            timeInfo = String.Format("All tasks successfully completed in: {0}",
                                this.TimeElapsed.ToString());
                        }
                        else
                        {
                            timeInfo = String.Format("Successfully completed in: {0}",
                                this.TimeElapsed.ToString());
                        }
                        logger.WriteLine(timeInfo, BuildLoggerLevel.Info);
                    }
                }
                else
                {
                    if (!this.IsMultiSteps)
                    {
                        if (logger != null)
                        {
                            logger.WriteLine(this.GetType().Name + ": An error occurred in this build step.", BuildLoggerLevel.Error);
                        }
                    }
                }
            }
            finally
            {
                if (logger != null && !String.IsNullOrEmpty(_title))
                {
                    logger.WriteLine(_title, BuildLoggerLevel.Ended);
                }
            }

            return mainResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual bool OnBeforeExecute(BuildContext context)
        {
            if (_beforeSteps == null || _beforeSteps.Enabled == false)
            {
                return true;
            }

            return _beforeSteps.Execute();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual bool OnReplaceExecute(BuildContext context)
        {
            if (_replaceSteps == null || _replaceSteps.Enabled == false)
            {
                return true;
            }

            return _replaceSteps.Execute();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual bool OnAfterExecute(BuildContext context)
        {
            if (_afterSteps == null || _afterSteps.Enabled == false)
            {
                return true;
            }

            return _afterSteps.Execute();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFile"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnStartTiming()
        {
            if (_stepTimer == null)
            {
                _stepTimer = new Stopwatch();
            }
            else
            {
                _stepTimer.Reset();
            }

            _stepTimer.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnStopTiming()
        {
            if (_stepTimer == null)
            {
                return;
            }

            _stepTimer.Stop();
        }

        protected virtual void OnInsert(BuildStep insertStep, 
            BuildInsertType insertType)
        {
            if (insertStep == null || insertType == BuildInsertType.None)
            {
                return;
            }

            switch (insertType)
            {
                case BuildInsertType.None:
                    break;
                case BuildInsertType.Before:
                    if (_beforeSteps != null)
                    {
                        _beforeSteps.Add(insertStep);
                    }
                    break;
                case BuildInsertType.Replace:
                    if (_replaceSteps != null)
                    {
                        _replaceSteps.Add(insertStep);
                    }
                    break;
                case BuildInsertType.After:
                    if (_afterSteps != null)
                    {
                        _afterSteps.Add(insertStep);
                    }
                    break;
            }
        }

        #endregion

        #region IDisposable Members

        /// <overloads>
        /// This performs build step tasks associated with freeing, releasing, or 
        /// resetting unmanaged resources.
        /// </overloads>
        /// <summary>
        /// This performs build step tasks associated with freeing, releasing, or 
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This cleans up any resources being used.
        /// </summary>
        /// <param name="disposing">
        /// This is <see langword="true"/> if managed resources should be 
        /// disposed; otherwise, <see langword="false"/>.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion
    }
}
