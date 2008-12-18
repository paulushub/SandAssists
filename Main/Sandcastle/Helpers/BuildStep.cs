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
    /// The build step derives from <see cref="MarshalByRefObject"/> so that any of its 
    /// derivative can be instantiated in its own app domain.
    /// </para>
    /// <para>
    /// In a build process, all the valid build steps will run except when the
    /// <see cref="BuildStep.Enabled"/> property is set to <see langword="false"/>.
    /// </para>
    /// </remarks>
    public abstract class BuildStep : MarshalByRefObject, IDisposable
    {
        #region Private Fields

        private bool      _isEnabled;
        private bool      _isInitialized;
        private bool      _continueOnError;
        private string    _name;
        private string    _message;
        private string    _description;
        private string    _workingDir;

        private Stopwatch _stepTimer;

        private BuildStep _preStep;
        private BuildStep _postStep;
        private BuildLoggerVerbosity _verbosity;
        private Dictionary<string, string> _properties;

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
            _isEnabled  = true;
            _properties = new Dictionary<string, string>(
                StringComparer.CurrentCultureIgnoreCase);
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
            : this()
        {           
            BuildExceptions.NotNullNotEmpty(name, "name");
            _workingDir = workingDir;
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
        /// An enumeration of the type <see cref="BuildStepType"/> specifyig the type
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
            set
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
        /// Gets or sets the message displayed by this build step.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the message displayed by this
        /// build step.
        /// </value>
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
        /// For build steps thatn cannot be cancelled, the steps are expected to
        /// monitor the state of the <see cref="BuildContext.IsCancelled"/> property
        /// and response automatically (cancelling the task themselves), where possible.
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
        /// Gets or sets the pre-build step for this build step, the pre-build step is 
        /// the build task that will be executed before this build step.
        /// </summary>
        /// <value>
        /// An instance of the <see cref="BuildStep"/> class specifying the task that
        /// must be executed before the task defined by this build step.
        /// </value>
        /// <remarks>
        /// The pre-build step can be defined using the multi-step, 
        /// <see cref="BuildMultiStep"/>, if the execution of several pre-build 
        /// steps is required.
        /// </remarks>
        /// <seealso cref="BuildStep.PostStep"/>
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

        /// <summary>
        /// Gets or sets the post-build step for this build step, the post-build step 
        /// defined the build task that will be executed after this build step.
        /// </summary>
        /// <value>
        /// An instance of the <see cref="BuildStep"/> class specifying the task that
        /// must be executed after the task defined by this build step.
        /// </value>
        /// <remarks>
        /// The post-build step can be defined using the multi-step, 
        /// <see cref="BuildMultiStep"/>, if the execution of several post-build 
        /// steps is required.
        /// </remarks>
        /// <seealso cref="BuildStep.PreStep"/>
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
                BuildExceptions.NotNullNotEmpty(key, "key");

                string strValue = String.Empty;
                if (_properties.TryGetValue(key, out strValue))
                {
                    return strValue;
                }

                return null;
            }
            set
            {
                BuildExceptions.NotNullNotEmpty(key, "key");

                bool bContains = _properties.ContainsKey(key);

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
                if (_properties != null)
                {
                    Dictionary<string, string>.KeyCollection keyColl
                        = _properties.Keys;

                    return keyColl;
                }

                return null;
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
                if (_properties != null)
                {
                    Dictionary<string, string>.ValueCollection valueColl
                        = _properties.Values;

                    return valueColl;
                }

                return null;
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
        /// <returns>
        /// This returns <see langword="true"/> if the build step is successfully
        /// initialized and ready for execution; otherwise, it returns 
        /// <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="context"/> is <see langword="null"/>.
        /// </exception>
        /// <seealso cref="BuildStep.IsInitialized"/>
        /// <seealso cref="BuildStep.Uninitialize()"/>
        public virtual bool Initialize(BuildContext context)
        {                        
            BuildExceptions.NotNull(context, "context");

            _isInitialized = false;
            if (_preStep != null && _preStep.Enabled)
            {
                _isInitialized = _preStep.Initialize(context);
                if (_isInitialized == false)
                {
                    return _isInitialized;
                }
            }
            if (_postStep != null && _postStep.Enabled)
            {
                _isInitialized = _postStep.Initialize(context);
                if (_isInitialized == false)
                {
                    return _isInitialized;
                }
            }

            _context = context;
            if (_verbosity == BuildLoggerVerbosity.None)
            {
                _verbosity = _context.Settings.Verbosity;
            }

            _isInitialized = true;

            return _isInitialized;
        }

        /// <summary>
        /// This provides the uninitialization process of this build step.
        /// </summary>
        /// <remarks>
        /// It provides the build step the way to clean up resources allocated
        /// during the initialization and execution of the build step.
        /// </remarks>
        /// <seealso cref="BuildStep.IsInitialized"/>
        /// <seealso cref="BuildStep.Initialize(BuildContext)"/>
        public virtual void Uninitialize()
        {
            if (_preStep != null && _preStep.Enabled)
            {
                _preStep.Uninitialize();
            }
            if (_postStep != null && _postStep.Enabled)
            {
                _postStep.Uninitialize();
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
            if (_isInitialized == false)
            {
                throw new BuildException("The build step is not initialized");
            }
            if (_context == null)
            {
                throw new BuildException(
                    "There is no build context associated with this build step.");
            }

            if (this.PreExecute(_context) == false)
            {
                if (_preStep.ContinueOnError == false)
                {
                    return false;
                }
            }

            BuildLogger logger = _context.Logger;
            if (logger != null && String.IsNullOrEmpty(_message) == false)
            {
                logger.WriteLine(_message, BuildLoggerLevel.Started);
            }

            this.StartTiming();

            bool mainResult = this.MainExecute(_context);

            this.StopTiming();

            if (!this.IsMultiSteps)
            {
                if (mainResult)
                {
                    if (logger != null)
                    {
                        string timeInfo = String.Format("Successfully Completed in: {0}",
                            this.TimeElapsed.ToString());
                        logger.WriteLine(timeInfo, BuildLoggerLevel.Info);
                    }
                }
                else
                {
                    if (logger != null)
                    {
                        logger.WriteLine("Step failed.", BuildLoggerLevel.Error);
                    }
                }
            }

            if (logger != null && String.IsNullOrEmpty(_message) == false)
            {
                logger.WriteLine(_message, BuildLoggerLevel.Ended);
            }

            if (mainResult == false)
            {
                if (this.ContinueOnError == false)
                {
                    return false;
                }
            }

            if (this.PostExecute(_context) == false)
            {
                if (_postStep.ContinueOnError == false)
                {
                    return false;
                }
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
        public virtual bool Cancel()
        {
            return false;
        }

        #region Properties Methods

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
            BuildExceptions.NotNullNotEmpty(key, "key");

            _properties.Remove(key);
        }

        /// <summary>
        /// This removes all keys and values from the <see cref="BuildSettings"/>.
        /// </summary>
        public void ClearProperties()
        {
            if (_properties.Count == 0)
            {
                return;
            }

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
            BuildExceptions.NotNullNotEmpty(key, "key");

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
            if (String.IsNullOrEmpty(key))
            {
                return false;
            }

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
        protected abstract bool MainExecute(BuildContext context);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual bool PreExecute(BuildContext context)
        {
            if (_preStep == null || _preStep.Enabled == false)
            {
                return true;
            }

            return _preStep.Execute();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual bool PostExecute(BuildContext context)
        {
            if (_postStep == null || _postStep.Enabled == false)
            {
                return true;
            }

            return _postStep.Execute();
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
        protected virtual void StartTiming()
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
        protected virtual void StopTiming()
        {
            if (_stepTimer == null)
            {
                return;
            }

            _stepTimer.Stop();
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