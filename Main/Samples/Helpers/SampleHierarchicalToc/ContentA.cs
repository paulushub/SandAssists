using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Company
{
    /// <summary>
    /// This is test content class A. 
    /// </summary>
    /// <tocexclude/>
    public class ContentA
    {
    }

    /// <summary>
    /// A test interface.
    /// </summary>
    /// <tocexclude/>
    public interface IContentA
    {           
    }

    /// <summary>
    /// A test enumeration.
    /// </summary>
    /// <tocexclude/>
    public enum EnumContentA
    {
        /// <summary>
        /// The value of none.
        /// </summary>
        None = 0
    }

    /// <summary>
    /// A test delegate.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    /// <tocexclude/>
    public delegate void TestDelegateHandler(object sender, EventArgs args);

    /// <summary>
    /// A test structure.
    /// </summary>
    /// <tocexclude/>
    public struct AStruct
    {
        private int _testItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="AStruct"/>.
        /// </summary>
        /// <param name="item">Input parameter.</param>
        public AStruct(int item)
        {
            _testItem = item;
        }

        /// <summary>
        /// Gets the value of the item.
        /// </summary>
        /// <value>An integer.</value>
        public int Item
        {
            get
            {
                return _testItem;
            }
        }
    }
}
