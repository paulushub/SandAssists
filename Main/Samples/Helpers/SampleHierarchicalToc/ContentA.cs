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

        public AStruct(int item)
        {
            _testItem = item;
        }

        public int Item
        {
            get
            {
                return _testItem;
            }
        }
    }
}
