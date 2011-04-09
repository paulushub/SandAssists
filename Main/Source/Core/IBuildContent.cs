using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Sandcastle
{
    public interface IBuildContent : ISupportInitialize, INotifyPropertyChanged, ICloneable, IXmlSerializable
    {   
        #region Public Events

        event EventHandler ModifiedChanged;

        #endregion

        #region Public Properties

        bool IsInitialized
        {
            get;
        }

        bool Modified
        {
            get;
            set;
        }

        #endregion

        #region Public Methods

        void ItemModified(IBuildItem item);

        #endregion
    }
}
