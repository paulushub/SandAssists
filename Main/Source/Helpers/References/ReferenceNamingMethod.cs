using System;

namespace Sandcastle.References
{
    /// <summary>
    /// This specifies the naming method used for the reference or API item names.
    /// </summary>
    [Serializable]
    public enum ReferenceNamingMethod
    {       
        /// <summary>
        /// Indicates a hashed name obtained by converting the hashed code of the Guid
        /// name to hexadecimal form.
        /// </summary>
        HashedName = 0,
        /// <summary>
        /// This indicates using a naming method based on dynamically created
        /// <see cref="System.Guid"/> values.
        /// </summary>
        Guid       = 1,
        /// <summary>
        /// This indicates using a naming method based on the full member name of
        /// the API item.
        /// </summary>
        MemberName = 2
    }
}
