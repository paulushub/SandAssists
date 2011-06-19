using System;
using System.Collections.Generic;
using System.Text;

namespace TestLibrary
{
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
