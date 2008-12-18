using System;

namespace Sandcastle
{
    /// <summary>
    /// This specifies the type of the unit of execution or build step.
    /// </summary>
    /// <seealso cref="BuildStep"/>
    [Serializable]
    public enum BuildStepType
    {
        None        = 0,
        InitializeReferences = 1,
        InitializeConceptual = 2,
        CopyResources = 2,
        CopyPresentations = 3,
        CopyDirectories = 4,
        DeleteDirectories = 5,
        CreateDirectories = 4,
        MoveDirectories  = 5,
        CleanIntermediates = 5,
        CompileHxs = 6,
        CompileChm = 7,
        AssembleReferences = 8,
        AssembleConceptual = 9,
        FixChmDbcs = 10,
        MergeToc = 11,
        ApplyDocumentModel = 14,
        ApplyNamingMethod = 15,
        GenerateReflection = 1,
        GenerateReferenceToc = 13,
        GenerateConceptualToc = 13,
        GenerateHxsProject = 13,
        GenerateChmProject = 14,
        GenerateHtmProject = 15,
        GenerateAspxProject = 16,
        CreateReferenceManifest = 14,
        CreateConceptualManifest = 14,
        StartChmViewer = 17,
        StartHxsViewer = 18,
        CloseChmViewer = 19,
        CloseHxsViewer = 20,
        Custom      = 30,
    }
}
