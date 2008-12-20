using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Sandcastle.References;
using Sandcastle.Conceptual;

namespace Sandcastle.Steps
{
    public class StepAssembler : StepProcess
    {
        #region Private Fields

        private BuildGroup _group;

        #endregion

        #region Constructors and Destructor

        public StepAssembler()
        {
        }

        public StepAssembler(string workingDir, string arguments)
            : base(workingDir, arguments)
        {
        }

        public StepAssembler(string workingDir, string fileName, string arguments)
            : base(workingDir, fileName, arguments)
        {
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

        protected override bool MainExecute(BuildContext context)
        {
            if (_group == null)
            {
                throw new BuildException(
                    "The build group for this step is required.");
            }

            if (_group.GroupType == BuildGroupType.Reference)
            {
                if (!CreateConfiguration((ReferenceGroup)_group))
                {
                    return false;
                }
            }
            else if (_group.GroupType == BuildGroupType.Conceptual)
            {
                if (!CreateConfiguration((ConceptualGroup)_group))
                {
                    return false;
                }
            }

            return base.MainExecute(context);
        }

        protected virtual bool CreateConfiguration(ReferenceGroup group)
        {
            BuildExceptions.NotNull(group, "group");

            BuildContext context     = this.Context;

            BuildLogger logger       = context.Logger;
            BuildSettings settings   = context.Settings;
            BuildStyle outputStyle   = settings.Style;
            BuildStyleType styleType = outputStyle.StyleType;

            string workingDir = settings.WorkingDirectory;
            string configDir = settings.ConfigurationDirectory;
            if (String.IsNullOrEmpty(workingDir))
            {
                if (logger != null)
                {
                    logger.WriteLine(
                        "The working directory is required, it is not specified.",
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            string configFile      = String.Empty;
            string finalConfigFile = String.Empty;
            if (String.IsNullOrEmpty(configDir) == false && 
                Directory.Exists(configDir))
            {
                configFile = Path.Combine(configDir, styleType.ToString() + ".config");
                finalConfigFile = Path.Combine(workingDir, group["$ConfigurationFile"]);
            }
            if (File.Exists(configFile) == false)
            {
                configFile = String.Empty;
            }

            ReferenceConfigurator assembler = new ReferenceConfigurator();

            try
            {
                assembler.Initialize(context);

                // 3. Configure the build assembler...
                if (!String.IsNullOrEmpty(configFile))
                {
                    assembler.Configure(group, configFile, finalConfigFile);
                }
            }
            finally
            {
                if (assembler != null)
                {
                    assembler.Uninitialize();
                }
            }

            return true;
        }

        protected virtual bool CreateConfiguration(ConceptualGroup group)
        {
            BuildExceptions.NotNull(group, "group");

            BuildContext context   = this.Context;
            BuildLogger logger     = context.Logger;
            BuildSettings settings = context.Settings;

            string workingDir = settings.WorkingDirectory;
            string configDir  = settings.ConfigurationDirectory;
            if (String.IsNullOrEmpty(workingDir))
            {
                if (logger != null)
                {
                    logger.WriteLine(
                        "The working directory is required, it is not specified.",
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            ConceptualConfigurator assembler = new ConceptualConfigurator();

            try
            {
                assembler.Initialize(context);

                string configFile  = String.Empty;
                string finalConfig = String.Empty;
                if (!String.IsNullOrEmpty(configDir) && Directory.Exists(configDir))
                {
                    configFile  = Path.Combine(configDir,  "Conceptual.config");
                    finalConfig = Path.Combine(workingDir, group["$ConfigurationFile"]);
                }
                if (File.Exists(configFile) == false)
                {
                    configFile = String.Empty;
                }

                // 1. Configure the build assembler...
                if (!String.IsNullOrEmpty(configFile))
                {
                    assembler.Configure(group, configFile, finalConfig);
                }

            }
            finally
            {
                if (assembler != null)
                {
                    assembler.Uninitialize();
                }
            }

            return true;
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
