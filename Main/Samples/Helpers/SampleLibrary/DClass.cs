using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace TestLibrary
{
    /// <summary>
    /// This is the summary for the <c>DClass</c> class.
    /// </summary>
    /// <remarks>
    /// The test remarks. Testing overload link: <see cref="O:TestLibrary.DClass.BMethod"/>
    /// </remarks>
    /// <seealso cref="Overload:TestLibrary.DClass.BMethod"/>
    /// <seealso cref="Overload:TestLibrary.DClass.Add"/>
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

        [BrowsableAttribute(true)]
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool BMethod()
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool BMethod(string name)
        {
            return false;
        }

        public void Add(string name)
        {

        }

        public void Add(string name, string socket)
        {

        }

        public void Add(string socket, DateTime robotpart)
        {

        }

        public void Add(string name, string socket, DateTime robotpart)
        {

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
