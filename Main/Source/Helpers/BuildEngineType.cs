using System;

namespace Sandcastle
{
    /// <summary>
    /// This specifies the type of the build engine.
    /// </summary>
    /// <remarks>
    /// Currently only two build engines are provided, more will be provided in
    /// future releases.
    /// </remarks>
    [Serializable]
    public enum BuildEngineType
    {
        /// <summary>
        /// Indicates the API or references build engine.
        /// <para>
        /// This is implemented by <see cref="Sandcastle.References.ReferenceEngine"/>
        /// </para>
        /// </summary>
        Reference  = 0,
        /// <summary>
        /// Indicates the conceptual topic build engine.
        /// <para>
        /// This is implemented by <see cref="Sandcastle.Conceptual.ConceptualEngine"/>
        /// </para>
        /// </summary>
        Conceptual = 1,
        /// <summary>
        /// This indicates a custom build engine.
        /// </summary>
        Custom     = 2
    }
}
