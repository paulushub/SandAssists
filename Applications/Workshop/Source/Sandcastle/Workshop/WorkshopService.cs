using System;

using ICSharpCode.Core;
using PropertyBag = ICSharpCode.Core.Properties;

namespace Sandcastle.Workshop
{
    public static class WorkshopService
    {
        #region Public Constants

        public const string WorkshopOptions        = "Workshop.Options";

        public const string ShowStartPageName      = "ShowStartPage";
        public const string ExpandStartPageAssistantName = "CloseOnProjectLoad";
        public const string CloseOnProjectLoadName = "CloseOnProjectLoad";

        #endregion

        #region Private Static Fields

        private static PropertyBag properties;

        #endregion

        #region Static Constructor

        static WorkshopService()
        {
            properties = PropertyService.Get(WorkshopOptions, new PropertyBag());
        }

        #endregion

        #region Public Static Events

        public static event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                properties.PropertyChanged += value;
            }
            remove
            {
                properties.PropertyChanged -= value;
            }
        }

        #endregion

        #region Public Static Properties

        public static bool CloseOnProjectLoad
        {
            get
            {
                return properties.Get(CloseOnProjectLoadName, true);
            }
            set
            {
                properties.Set(CloseOnProjectLoadName, value);
            }
        }

        public static bool ShowStartPage
        {
            get
            {
                return properties.Get(ShowStartPageName, true);
            }
            set
            {
                properties.Set(ShowStartPageName, value);
            }
        }

        #endregion

        #region Internal Static Properties

        internal static bool ExpandStartPageAssistant
        {
            get
            {
                return properties.Get(ExpandStartPageAssistantName, true);
            }
            set
            {
                properties.Set(ExpandStartPageAssistantName, value);
            }
        }

        #endregion
    }
}
