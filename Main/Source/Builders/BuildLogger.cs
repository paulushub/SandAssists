using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Builders
{
    public sealed class BuildLogger : HelpLogger
    {
        #region Private Fields

        private List<HelpLogger> _listLoggers;

        #endregion

        #region Constructors and Destructor

        public BuildLogger()
        {
            _listLoggers = new List<HelpLogger>();
        }

        public BuildLogger(string logFile)
            : base(logFile)
        {
            _listLoggers = new List<HelpLogger>();
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

        #endregion

        #region Public Methods

        public override void Initialize()
        {
            base.Initialize();

            if (_listLoggers != null)
            {
                int itemCount = _listLoggers.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    HelpLogger logger = _listLoggers[i];
                    logger.Initialize();
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
                    HelpLogger logger = _listLoggers[i];
                    logger.Uninitialize();
                }

                _listLoggers.Clear();
            }
        }

        public void Add(HelpLogger logger)
        {
            if (_listLoggers != null)
            {
                _listLoggers.Add(logger);
            }
        }

        public void Remove(HelpLogger logger)
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
                HelpLogger logger = _listLoggers[i];
                logger.WriteLine();
            }
        }

        public override void Write(Exception ex, HelpLoggerLevel level)
        {
            if (ex == null)
            {
                return;
            }
            this.Write(ex.ToString(), level);
        }

        public override void Write(string outputText, HelpLoggerLevel level)
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
                HelpLogger logger = _listLoggers[i];
                logger.Write(outputText, level);
            }
        }

        public override void WriteLine(Exception ex, HelpLoggerLevel level)
        {
            if (ex == null)
            {
                return;
            }

            this.WriteLine(ex.ToString(), level);
        }

        public override void WriteLine(string outputText, HelpLoggerLevel level)
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
                    HelpLogger logger = _listLoggers[i];
                    logger.WriteLine();
                }
            }
            else
            {
                for (int i = 0; i < itemCount; i++)
                {
                    HelpLogger logger = _listLoggers[i];
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
