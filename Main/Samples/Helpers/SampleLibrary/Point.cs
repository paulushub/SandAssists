using System;

namespace TestLibrary
{
    /// <summary>
    /// This is a point class.
    /// </summary>
    /// <remarks>
    /// <code language="c#">
    ///using System;
    ///using System.Text;
    ///using System.Collections.Generic;
    ///using TestLibrary;
    /// 
    ///namespace TestSample
    ///{
    ///    class Program
    ///    {
    ///        static void Main(string[] args)
    ///        {
    ///            Point pt = new Point(10, 20);
    ///            Console.WriteLine(String.Format("[{0}, {1}]", pt.X, pt.Y));
    ///        }
    ///    }
    ///}
    /// </code>
    /// </remarks>
    public class Point
    {
        private int _x;
        private int _y;

        /// <overloads>
        /// Initializes a new instance of the <see cref="Point"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> class with
        /// the default parameters.
        /// </summary>
        public Point()
        {   
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> class with
        /// the specified x- and y- coordinates.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public Point(int x, int y)
        {   
        }

        /// <summary>
        /// Gets or sets the x coordinate.
        /// </summary>
        /// <value>
        /// A <see cref="System.Int32"/> specifying the x coordinate.
        /// </value>
        public virtual int X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
            }
        }

        /// <summary>
        /// Gets or sets the y coordinate.
        /// </summary>
        /// <value>
        /// A <see cref="System.Int32"/> specifying the y coordinate.
        /// </value>
        public virtual int Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
            }
        }

        /// <overloads>
        /// Moves or offsets the current position by a specified amount.
        /// </overloads>
        /// <summary>
        /// Moves or offsets the current position by the specified amount in both
        /// x- and y-axis.
        /// </summary>
        /// <param name="value">The amount for both x- and y-axis.</param>
        public virtual void Translate(int value)
        {   
        }

        /// <summary>
        /// Moves or offsets the current position by the specified x- and y-values.
        /// </summary>
        /// <param name="x">The x-offset.</param>
        /// <param name="y">The y-offset</param>
        public virtual void Translate(int x, int y)
        {   
        }
    }
}
