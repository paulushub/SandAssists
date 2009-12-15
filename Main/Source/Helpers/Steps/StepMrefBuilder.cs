using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Sandcastle.References;

namespace Sandcastle.Steps
{
    public class StepMrefBuilder : StepProcess
    {
        #region Private Fields

        private BuildGroup _group;

        #endregion

        #region Constructors and Destructor

        public StepMrefBuilder()
        {
            this.Message = "Reflection Builder Tool";
        }

        public StepMrefBuilder(string workingDir, string arguments)
            : base(workingDir, arguments)
        {
            this.Message = "Reflection Builder Tool";
        }

        public StepMrefBuilder(string workingDir, string fileName, string arguments)
            : base(workingDir, fileName, arguments)
        {
            this.Message = "Reflection Builder Tool";
        }

        #endregion

        #region Public Properties

        public BuildGroup Group
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

        protected override bool MainExecute(BuildContext context)
        {
            if (_group == null)
            {
                throw new BuildException(
                    "The build group for this step is required.");
            }

            ReferenceGroup group = _group as ReferenceGroup;
            if (group == null)
            {
                return false;
            }

            BuildLogger logger     = context.Logger;
            BuildSettings settings = context.Settings;
            BuildStyle outputStyle = settings.Style;
                                           
            string workingDir = context.WorkingDirectory;
            string configDir  = settings.ConfigurationDirectory;

            if (String.IsNullOrEmpty(workingDir))
            {
                if (logger != null)
                {
                    logger.WriteLine("The working directory is required, it is not specified.",
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            if (!this.CreateConfiguration(context, group, workingDir, configDir))
            {
                return false;
            }

            return base.MainExecute(context);
        }

        #endregion

        #region CreateConfiguration Method

        protected virtual bool CreateConfiguration(BuildContext context,
            ReferenceGroup group, string workingDir, string configDir)
        {
            BuildExceptions.NotNull(context, "context");
            BuildExceptions.NotNull(group, "group");

            string refBuilder         = String.Empty;
            string refBuilderDefAttrs = String.Empty;
            string finalRefBuilder    = String.Empty;
            if (String.IsNullOrEmpty(configDir) == false && 
                Directory.Exists(configDir))
            {
                refBuilder         = Path.Combine(configDir,  "MRefBuilder.config");
                refBuilderDefAttrs = Path.Combine(configDir,  "MRefBuilder.xml");
                finalRefBuilder    = Path.Combine(workingDir,
                    group["$ReflectionBuilderFile"]);
            }
            if (File.Exists(refBuilder) == false)
            {
                refBuilder = String.Empty;
            }

            ReferenceFilterConfigurator filterer = new ReferenceFilterConfigurator();

            try
            {
                filterer.Initialize(context, refBuilderDefAttrs);

                if (!String.IsNullOrEmpty(refBuilder))
                {
                    filterer.Configure(group, refBuilder, finalRefBuilder);
                }
            }
            finally
            {
                if (filterer != null)
                {
                    filterer.Uninitialize();
                }
            }

            return true;
        }

        #endregion

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
