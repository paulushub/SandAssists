// Copyright ｩ Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;

namespace Sandcastle.ReflectionData
{
    // The basic object model here is this:
    //  * Target objects represent files that can be targeted by a reference link
    //  * Different child objects of Target represent different sorts of API targets: Namespace, Type, Member, etc.
    //  * Targets are stored in a TargetCollection
    // To indicate relationships between targets (e.g. a Method takes a particular type parameter), we
    // introduce another set of classes:
    //  * Reference objects refer to a specific target
    //  * Objects like SpecializedTypeReference and ArrayTypeReference that represent decorated types
    // There are two ways to construct such objects:
    //  * XML from a reflection information file defines Target and Reference objects. XmlUtilities does this.
    //  * Code entity reference strings construct Reference objects. CerUtilities does this.
    // Finally, we need a way to write the link text corresponding to a reference:
    //  * LinkTextResolver contains routines that, given a reference, writes the corresponding link text

    // all arguments of public methods are verified

    // The fact that the creation methods (via XML or CER strings) for references and their rendering methods
    // are separated from the declarations of the reference types goes against OO principals. (The consequent
    // absence of virtual methods also makes for a lot of ugly casting to figure out what method to call.)
    // But there is a reason for it: I wanted all the code that interpreted XML together, all the code that
    // interpreted CER strings together, and all the code that did link text rendering together, and I wanted
    // them all separator from each other. I believe this is extremely important for maintainability. It may
    // be possible to leverage partial classes to do this in a more OO fashion.

    // contains a collection of targets

    public abstract class TargetCollection
    {
        #region Public Fields

        public bool RecentLinkTypeIsMsdn;
        public ReferenceLinkType RecentLinkType;

        #endregion

        #region Constructor and Destructor

        protected TargetCollection()
        {
        }

        #endregion

        #region Public Properties

        // read the collection

        public abstract int Count
        {
            get;
        }

        public abstract Target this[string id]
        {
            get;
        }

        #endregion

        #region Public Methods

        public abstract bool Contains(string id);

        #endregion
    }
}
