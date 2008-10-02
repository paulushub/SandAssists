using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle
{
    [Serializable]
    public class BuildMultiStep : BuildStep
    {
        #region Private Fields

        private List<BuildStep> _listSteps;

        #endregion

        #region Constructors and Destructor

        public BuildMultiStep()
        {
            _listSteps = new List<BuildStep>();
        }

        public BuildMultiStep(IList<BuildStep> _listSteps)
        {
            if (_listSteps != null && _listSteps.Count != 0)
            {
                _listSteps = new List<BuildStep>(_listSteps);
            }
            else
            {
                _listSteps = new List<BuildStep>();
            }
        }

        public BuildMultiStep(BuildMultiStep source)
            : base(source)
        {
            _listSteps = source._listSteps;
        }

        #endregion

        #region Public Properties

        public override bool IsMultiSteps
        {
            get
            {
                return true;
            }
        }

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

        public void Remove(BuildStep step)
        {
            BuildExceptions.NotNull(step, "step");

            if (_listSteps == null || _listSteps.Count == 0)
            {
                return;
            }

            _listSteps.Remove(step);
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
                            "An error occurred when initializing the step = " + i.ToString(),
                            BuildLoggerLevel.Error);

                        initResult = false;
                        break;
                    }
                }
            }

            return initResult;
        }

        public override bool Uninitialize(BuildContext context)
        {
            bool unInitResult = base.Uninitialize(context);
            if (unInitResult == false)
            {
                return unInitResult;
            }

            if (_listSteps == null || _listSteps.Count == 0)
            {
                return unInitResult;
            }

            BuildLogger logger = context.Logger;

            int stepCount = _listSteps.Count;

            for (int i = 0; i < stepCount; i++)
            {
                BuildStep buildStep = _listSteps[i];
                if (buildStep != null)
                {
                    if (buildStep.Uninitialize(context) == false)
                    {
                        logger.WriteLine(
                            "An error occurred when uninitializing the step = " + i.ToString(),
                            BuildLoggerLevel.Error);

                        unInitResult = false;
                    }
                }
            }

            return unInitResult;
        }

        #endregion

        #region Protected Methods

        protected override bool MainExecute(BuildEngine engine)
        {
            BuildLogger logger = engine.Logger;
            BuildContext context = this.Context;

            bool buildResult = false;

            if (context == null || _listSteps == null || _listSteps.Count == 0)
            {
                return buildResult;
            }

            int stepCount = _listSteps.Count;

            try
            {
                buildResult = true;

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
                    
                    if (buildStep.Execute() == false)
                    {
                        if (logger != null)
                        {
                            logger.WriteLine(
                                "An error occurred in the step = " + i.ToString(),
                                BuildLoggerLevel.Error);
                        }
                        context.StepError(buildStep);

                        buildResult = false;
                        break;
                    }
                    
                    context.StepEnds(buildStep);

                    if (logger != null)
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

        #region ICloneable Members

        public override BuildStep Clone()
        {
            BuildMultiStep buildStep = new BuildMultiStep(this);
            string workingDir = this.WorkingDirectory;
            if (workingDir != null)
            {
                buildStep.WorkingDirectory = String.Copy(workingDir);
            }

            return buildStep;
        }

        #endregion
    }
}
