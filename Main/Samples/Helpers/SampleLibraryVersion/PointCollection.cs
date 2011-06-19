using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TestLibrary
{
    /// <summary>
    /// A collection of the <see cref="Point"/> objects.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public class PointCollection<T> : Collection<T>
        where T : Point
    {
        /// <overloads>
        /// Initializes a new instance of the <see cref="PointCollection{T}"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="PointCollection{T}"/> class.
        /// </summary>
        public PointCollection()
        {   
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PointCollection{T}"/> class as
        /// a wrapper of the specified list.
        /// </summary>
        /// <param name="list"></param>
        public PointCollection(IList<T> list)
            : base(list)
        {
        } 
    }
}
