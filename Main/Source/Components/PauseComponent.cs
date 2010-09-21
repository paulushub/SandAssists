using System;
using System.Threading;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components
{
    /// <summary>
    /// This component pause the initialization or normal processing for a specified
    /// period, and it is used for debugging.
    /// </summary>
    /// <remarks>
    /// The following is an example of the configuration, that will pause the 
    /// initialization of the components at the point of insertion for 5000 milliseconds
    /// and 3000 milliseconds during the running process, repeating this twice.
    /// <code lang="xml">
    /// <![CDATA[
    /// <component type="Sandcastle.Components.PauseComponent" assembly="Sandcastle.Components.dll">
    ///     <initPeriod>5000</initPeriod>
    ///     <runPeriod repeat="2">3000</runPeriod>
    /// </component>
    /// ]]>
    /// </code>
    /// </remarks>
    public sealed class PauseComponent : BuildComponentEx
    {
        #region Private Fields

        private int _initDeplay;
        private int _runDelay;
        private int _runDelayCount;
        private int _runDelayMax;

        #endregion

        #region Constructors and Destructor

        public PauseComponent(BuildAssembler assembler,
            XPathNavigator configuration)
            : base(assembler, configuration)
        {
            _initDeplay  = _runDelay = _runDelayCount = 0;
            _runDelayMax = 1;

            try
            {
                XPathNavigator itemNode = configuration.SelectSingleNode("initPeriod");
                if (itemNode != null && !String.IsNullOrEmpty(itemNode.Value))
                {
                    _initDeplay = Convert.ToInt32(itemNode.Value);
                }
                itemNode = configuration.SelectSingleNode("runPeriod");
                if (itemNode != null && !String.IsNullOrEmpty(itemNode.Value))
                {
                    _runDelay = Convert.ToInt32(itemNode.Value);
                    string runMax = itemNode.GetAttribute("repeat", String.Empty);
                    if (!String.IsNullOrEmpty(runMax))
                    {
                        _runDelayMax = Convert.ToInt32(runMax);
                    }
                }

                if (_initDeplay > 0)
                {
                    Thread.Sleep(_initDeplay);
                }
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, ex);
            }
        }

        #endregion

        #region Public Methods

        public override void Apply(XmlDocument document, string key)
        {               
            if (_runDelay > 0 && _runDelayCount < _runDelayMax)
            {
                _runDelayCount++;
                Thread.Sleep(_runDelay);
            }
        }

        #endregion
    }
}
