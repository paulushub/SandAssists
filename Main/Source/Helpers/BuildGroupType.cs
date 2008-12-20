using System;

namespace Sandcastle
{
    /// <summary>
    /// This specifies the type of the build group, which is a categorization of the
    /// build contents.
    /// </summary>
    /// <seealso cref="BuildGroup"/>
    [Serializable]
    public enum BuildGroupType
    {
        /// <summary>
        /// Indicates an unknown or unspecified build group.
        /// </summary>
        None       = 0,
        /// <summary>
        /// Indicates a reference or application programming interface (API) 
        /// build group, <see cref="Sandcastle.References.ReferenceGroup"/>.
        /// </summary>
        Reference  = 1,
        /// <summary>
        /// Indicates the conceptual topics build group,
        /// <see cref="Sandcastle.Conceptual.ConceptualGroup"/>
        /// </summary>
        Conceptual = 2
    }
}
