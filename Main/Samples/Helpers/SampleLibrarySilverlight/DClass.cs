using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace TestLibrary
{
    /// <summary>
    /// </summary>
    public class DClass : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public DClass()
        {   
        }

        public DClass(int testing)
        {   
        }

        ~DClass()
        {
        }

        [DefaultValueAttribute("Sample")]
        [CategoryAttribute("Testing")]
        public string AProperty
        {
            get
            {
                return null;
            }
            set
            {   
            }
        }

        public void AMethod()
        {   
        }

        public bool BMethod()
        {
            return false;
        }

        public bool BMethod(string name)
        {
            return false;
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion

        /// <include file='XML_include_tag.doc' path='MyDocs/MyMembers[@name="test"]/*' />
        public class DInnerClass
        {
            public DInnerClass()
            {                    
            }

            /// <include file='XML_include_tag.doc' path='MyDocs/MyMembers[@name="test1"]/*' />
            /// <include file='XML_include_tag.doc' path='MyDocs/MyMembers[@name="test2"]/*' />
            /// <exception cref="ArgumentNullException"></exception>
            public string InnerAProperty
            {
                get
                {
                    return null;
                }
                set
                {
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <exception cref=""></exception>
            public void InnerAMethod()
            {
            }

            public bool InnerBMethod()
            {
                return false;
            }

            public bool InnerBMethod(string name)
            {
                return false;
            }
        }
    }
}
