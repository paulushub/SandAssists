using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle
{
    /// <summary>
    /// This is a build step, which acts as a container for other build steps.
    /// It provides a means of grouping related or consecutive build steps.
    /// </summary>
    public class BuildMultiStep : BuildStep, ICollection<BuildStep>
    {
        #region Private Fields

        private List<string>    _listMessages;
        private List<BuildStep> _listSteps;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildMultiStep"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildMultiStep"/> class
        /// to the default properties or values.
        /// </summary>
        public BuildMultiStep()
        {
            _listSteps       = new List<BuildStep>();
            this.LogTimeSpan = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildMultiStep"/> class with the
        /// specified working directory.
        /// </summary>
        /// <param name="workingDir">
        /// A <see cref="System.String"/> containing the working directory.
        /// </param>
        public BuildMultiStep(string workingDir)
            : base(workingDir)
        {
            _listSteps       = new List<BuildStep>();
            this.LogTimeSpan = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildMultiStep"/> class with the
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
        public BuildMultiStep(string name, string workingDir)
            : base(name, workingDir)
        {
            _listSteps       = new List<BuildStep>();
            this.LogTimeSpan = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildMultiStep"/> class with
        /// the initial build steps specified.
        /// </summary>
        /// <param name="listSteps">
        /// A list, <see cref="IList{T}"/>, specifying the initial build steps of this
        /// multi-step build step.
        /// </param>
        public BuildMultiStep(IList<BuildStep> listSteps)
        {
            if (listSteps != null && listSteps.Count != 0)
            {
                _listSteps = new List<BuildStep>(listSteps);
            }
            else
            {
                _listSteps = new List<BuildStep>();
            }
            this.LogTimeSpan = false;
        }

        #endregion

        #region Public Properties

        public int Count
        {
            get
            {
                if (_listSteps != null)
                {
                    return _listSteps.Count;
                }

                return 0;
            }
        }

        public BuildStep this[int index]
        {
            get
            {
                if (_listSteps != null && index >= 0 && index < _listSteps.Count)
                {
                    return _listSteps[index];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this build step is a multi-step task.
        /// </summary>
        /// <value>
        /// This property will always return <see langword="true"/> since this is a 
        /// multi-step task.
        /// </value>
        public override bool IsMultiSteps
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a list of all the build steps currently contained in this multi-task
        /// step.
        /// </summary>
        /// <value>
        /// A list, <see cref="IList{T}"/>, of the <see cref="BuildStep"/> currently
        /// added to this container.
        /// <note type="caution">
        /// <para>
        /// This is a read-only list and cannot be modified.
        /// </para>
        /// </note>
        /// </value>
        public IList<BuildStep> Steps
        {
            get
            {
                if (_listSteps != null)
                {
                    return _listSteps.AsReadOnly();
                }

                return null;
            }
        }

        public IList<string> Messages
        {
            get
            {
                if (_listMessages != null)
                {
                    return _listMessages.AsReadOnly();
                }

                return null;
            }
        }

        #endregion

        #region Public Methdods

        public void Add(BuildStep step)
        {
            BuildExceptions.NotNull(step, "step");

            if (_listSteps == null)
            {
                _listSteps = new List<BuildStep>();
            }

            _listSteps.Add(step);
        }

        public void Add(BuildStep step, string message)
        {
            this.Add(step);
            if (message == null)
            {
                message = String.Empty;
            }

            if (_listMessages == null)
            {
                _listMessages = new List<string>();
            }

            _listMessages.Add(message);
        }

        public void Add(IList<BuildStep> steps)
        {
            BuildExceptions.NotNull(steps, "steps");

            if (steps.Count == 0)
            {
                return;
            }

            if (_listSteps == null)
            {
                _listSteps = new List<BuildStep>(steps);
            }
            else
            {
                _listSteps.AddRange(steps);
            }
        }

        public void Remove(int index)
        {
            if (_listSteps == null || _listSteps.Count == 0)
            {
                return;
            }

            _listSteps.RemoveAt(index);
        }

        public bool Remove(BuildStep step)
        {
            BuildExceptions.NotNull(step, "step");

            if (_listSteps == null || _listSteps.Count == 0)
            {
                return false;
            }

            return _listSteps.Remove(step);
        }

        public bool Contains(BuildStep step)
        {
            if (step == null || _listSteps == null || _listSteps.Count == 0)
            {
                return false;
            }

            return _listSteps.Contains(step);
        }

        public void Clear()
        {
            if (_listSteps == null || _listSteps.Count == 0)
            {
                return;
            }

            _listSteps.Clear();
        }

        public override bool Initialize(BuildContext context)
        {
            bool initResult = base.Initialize(context);
            if (initResult == false)
            {
                return initResult;
            }

            if (_listSteps == null || _listSteps.Count == 0)
            {
                return initResult;
            }

            BuildLogger logger = context.Logger;

            int stepCount = _listSteps.Count;

            for (int i = 0; i < stepCount; i++)
            {
                BuildStep buildStep = _listSteps[i];
                if (buildStep != null)
                {
                    if (buildStep.Initialize(context) == false)
                    {
                        logger.WriteLine(
                            "An error occurred when initializing the multi-step = " + i.ToString(),
                            BuildLoggerLevel.Error);

                        initResult = false;
                        break;
                    }
                }
            }

            return initResult;
        }

        public override void Uninitialize()
        {
            if (_listSteps == null || _listSteps.Count == 0)
            {
                base.Uninitialize();

                return;
            }

            BuildLogger logger = this.Context.Logger;

            int stepCount = _listSteps.Count;

            try
            {
                for (int i = 0; i < stepCount; i++)
                {
                    BuildStep buildStep = _listSteps[i];
                    if (buildStep != null)
                    {
                        buildStep.Uninitialize();
                    }
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.WriteLine(ex, BuildLoggerLevel.Error);
                }
            }

            base.Uninitialize();
        }

        #endregion

        #region Protected Methods

        protected override bool OnExecute(BuildContext context)
        {
            BuildLogger logger = context.Logger;

            bool buildResult = true;

            if (_listSteps == null || _listSteps.Count == 0)
            {
                if (logger != null)
                {
                    logger.WriteLine("This multi-step contains no build step.", 
                        BuildLoggerLevel.Warn);
                }
                return buildResult;
            }

            int stepCount = _listSteps.Count;

            try
            {
                for (int i = 0; i < stepCount; i++)
                {
                    BuildStep buildStep = _listSteps[i];
                    if (buildStep == null || buildStep.Enabled == false)
                    {
                        continue;
                    }
                    bool executeIt = context.StepStarts(buildStep);
                    if (executeIt == false)
                    {
                        continue;
                    }

                    if (_listMessages != null && _listMessages.Count == stepCount)
                    {
                        if (logger != null)
                        {
                            logger.WriteLine(_listMessages[i], BuildLoggerLevel.Info);
                        }
                    }

                    if (!buildStep.Execute())
                    {
                        if (logger != null)
                        {
                            logger.WriteLine(
                                "An error occurred in the multi-step = " + i.ToString(),
                                BuildLoggerLevel.Error);
                        }
                        context.StepError(buildStep);

                        if (!buildStep.ContinueOnError)
                        {
                            buildResult = false;
                            break;
                        }
                    }
                    
                    context.StepEnds(buildStep);

                    if (i != (stepCount - 1) && logger != null)
                    {
                        logger.WriteLine();
                    }
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.WriteLine(ex, BuildLoggerLevel.Error);
                }

                buildResult = false;
            }

            return buildResult;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// This cleans up any resources being used.
        /// </summary>
        /// <param name="disposing">
        /// This is <see langword="true"/> if managed resources should be 
        /// disposed; otherwise, <see langword="false"/>.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (_listSteps != null))
            {
                int stepCount = _listSteps.Count;

                try
                {
                    for (int i = 0; i < stepCount; i++)
                    {
                        BuildStep buildStep = _listSteps[i];
                        if (buildStep != null)
                        {
                            buildStep.Dispose();
                        }
                    }
                }
                catch
                {
                }
            }

            _listSteps = null;

            base.Dispose(disposing);
        }

        #endregion

        #region ICollection<BuildStep> Members 

        public void CopyTo(BuildStep[] array, int arrayIndex)
        {
            if (_listSteps != null)
            {
                _listSteps.CopyTo(array, arrayIndex);
            }
        }

        public bool IsReadOnly
        {
            get 
            {
                return false;
            }
        }

        #endregion

        #region IEnumerable<BuildStep> Members

        public IEnumerator<BuildStep> GetEnumerator()
        {
            if (_listSteps != null)
            {
                return _listSteps.GetEnumerator();
            }

            return null;
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            if (_listSteps != null)
            {
                return _listSteps.GetEnumerator();
            }

            return null;
        }

        #endregion
    }
}
