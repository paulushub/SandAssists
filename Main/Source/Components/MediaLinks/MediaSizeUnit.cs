using System;

namespace Sandcastle.Components.MediaLinks
{
    /// <summary>
    /// This specifies the units of measurement of sizes (width and height) of
    /// a multimedia documentation object.
    /// </summary>
    /// <remarks>
    /// These are CSS standard defined units for length, size, width and height.
    /// The commonly used units are <c>Pixels(px)</c>, <c>Points(pt)</c>, 
    /// <c>Percentages(%)</c> and <c>Ems(em)</c>.
    /// </remarks>
    [Serializable]
    public enum MediaSizeUnit
    {
        /// <summary>
        /// Indicates an unknown or unspecified unit of an element size.
        /// </summary>
        None    = 0,
        /// <summary>
        /// Indicates the size of the element in pixels, <c>px</c>.
        /// </summary>
        Pixel   = 1,
        /// <summary>
        /// Indicates the size of the element in points, <c>pt</c>.
        /// </summary>
        Point   = 2,
        /// <summary>
        /// Indicates the size of the element in percentages, <c>%</c>.
        /// </summary>
        Percent = 3,
        /// <summary>
        /// Indicates the size of the element in Ems or the height of the
        /// capital letter <c>M</c> in the scope font size, <c>em</c>.
        /// </summary>
        Em      = 4,
        /// <summary>
        /// Indicates the size of the element in picas or <c>1/6</c> of
        /// an inch, <c>pc</c>.
        /// </summary>
        Pica    = 5,
        /// <summary>
        /// Indicates the size of the element in Exes or the size of letter
        /// <c>X</c> in the scope font size, <c>ex</c>.
        /// </summary>
        Ex      = 6
    }
}
