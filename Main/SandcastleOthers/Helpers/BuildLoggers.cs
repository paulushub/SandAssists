using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle
{
    public sealed class BuildLoggers : BuildLogger
    {
        #region Private Fields

        private List<BuildLogger> _listLoggers;

        #endregion

        #region Constructors and Destructor

        public BuildLoggers()
        {
            _listLoggers = new List<BuildLogger>();
        }

        public BuildLoggers(string logFile)
            : base(logFile)
        {
            _listLoggers = new List<BuildLogger>();
        }

        #endregion

        #region Public Properties

        public int Count
        {
            get
            {
                if (_listLoggers != null)
                {
                    return _listLoggers.Count;
                }

                return 0;
            }
        }

        public BuildLogger this[int index]
        {
            get
            {
                if (_listLoggers != null)
                {
                    return _listLoggers[index];
                }

                return null;
            }
        }

        public IList<BuildLogger> Loggers
        {
            get
            {
                if (_listLoggers != null)
                {
                    return _listLoggers.AsReadOnly();
                }

                return null;
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(BuildSettings settings)
        {
            base.Initialize(settings);

            if (_listLoggers != null)
            {
                int itemCount = _listLoggers.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    BuildLogger logger = _listLoggers[i];
                    logger.Initialize(settings);
                }
            }
        }

        public override void Uninitialize()
        {
            base.Uninitialize();

            if (_listLoggers != null)
            {
                int itemCount = _listLoggers.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    BuildLogger logger = _listLoggers[i];
                    logger.Uninitialize();
                }

                _listLoggers.Clear();
            }
        }

        public void Add(BuildLogger logger)
        {
            if (_listLoggers != null)
            {
                _listLoggers.Add(logger);
            }
        }

        public void Remove(BuildLogger logger)
        {
            if (_listLoggers != null)
            {
                _listLoggers.Remove(logger);
            }
        }

        public void Clear()
        {
            if (_listLoggers != null)
            {
                int itemCount = _listLoggers.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    _listLoggers[i].Dispose();
                }

                _listLoggers.Clear();
            }
        }

        public override void WriteLine()
        {
            if (_listLoggers == null || this.Logging == false)
            {
                return;
            }

            int itemCount = _listLoggers.Count;
            for (int i = 0; i < itemCount; i++)
            {
                BuildLogger logger = _listLoggers[i];
                logger.WriteLine();
            }
        }

        public override void Write(Exception ex, BuildLoggerLevel level)
        {
            if (ex == null)
            {
                return;
            }
            this.Write(ex.ToString(), level);
        }

        public override void Write(string outputText, BuildLoggerLevel level)
        {
            if (String.IsNullOrEmpty(outputText))
            {
                return;
            }

            if (_listLoggers == null || this.Logging == false)
            {
                return;
            }

            int itemCount = _listLoggers.Count;
            for (int i = 0; i < itemCount; i++)
            {
                BuildLogger logger = _listLoggers[i];
                logger.Write(outputText, level);
            }
        }

        public override void WriteLine(Exception ex, BuildLoggerLevel level)
        {
            if (ex == null)
            {
                return;
            }

            this.WriteLine(ex.ToString(), level);
        }

        public override void WriteLine(string outputText, BuildLoggerLevel level)
        {
            if (_listLoggers == null || this.Logging == false)
            {
                return;
            }

            int itemCount = _listLoggers.Count;

            if (String.IsNullOrEmpty(outputText))
            {
                for (int i = 0; i < itemCount; i++)
                {
                    BuildLogger logger = _listLoggers[i];
                    logger.WriteLine();
                }
            }
            else
            {
                for (int i = 0; i < itemCount; i++)
                {
                    BuildLogger logger = _listLoggers[i];
                    logger.WriteLine(outputText, level);
                }
            }
        }

        #endregion

        #region Protected Methods

        protected override void Write(string outputText)
        {
        }

        protected override void WriteLine(string outputText)
        {
        }

        #endregion
    }
}
