using System;
using System.Collections.Generic;
using System.Text;

namespace TestLibrary
{
    /// <summary>
    /// This is a 3-dimensional point class.
    /// </summary>
    public class Point3D : Point
    {
        private int _z;

        /// <overloads>
        /// Initializes a new instance of the <see cref="Point3D"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="Point3D"/> class with
        /// the default parameters.
        /// </summary>
        public Point3D()
        {   
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Point3D"/> class with
        /// the specified x-, y- and z- coordinates.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <param name="z">The z coordinate.</param>
        public Point3D(int x, int y, int z)
            : base(x, y)
        {
            _z = z;
        }

        /// <inheritdoc/>
        public override int X
        {
            get
            {
                return base.X;
            }
            set
            {
                base.X = value;
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <value>
        /// <inheritdoc/>
        /// </value>
        public override int Y
        {
            get
            {
                return base.Y;
            }
            set
            {
                base.Y = value;
            }
        }

        /// <summary>
        /// Gets or sets the z coordinate.
        /// </summary>
        /// <value>
        /// A <see cref="System.Int32"/> specifying the z coordinate.
        /// </value>
        public int Z
        {
            get
            {
                return _z;
            }
            set
            {
                _z = value;
            }
        }
    }
}
