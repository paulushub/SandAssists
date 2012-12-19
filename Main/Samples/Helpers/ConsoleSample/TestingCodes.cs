using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Sandcastle;
using Sandcastle.ReflectionData;

namespace ConsoleSample
{
    class TestingCodes
    {
        //IList<Version> frameworkVersions = 
        //    BuildFrameworks.InstalledFrameworkVersions;

        //if (frameworkVersions != null && frameworkVersions.Count != 0)
        //{
        //    for (int i = 0; i < frameworkVersions.Count; i++)
        //    {
        //        Console.WriteLine(frameworkVersions[i].ToString());
        //    }

        //    Console.WriteLine("-------------");
        //    frameworkVersions = BuildFrameworks.InstalledSilverlightVersions;
        //    for (int i = 0; i < frameworkVersions.Count; i++)
        //    {
        //        Console.WriteLine(frameworkVersions[i].ToString());
        //    }

        //    return;
        //}

        //try
        //{
        //    TargetDictionaryBuilder targetDictionary =
        //        new TargetDictionaryBuilder();

        //    if (!targetDictionary.Exists)
        //    {
        //        Console.WriteLine("Please wait, building target lists...");
        //        targetDictionary.Build();
        //    }

        //    targetDictionary.Dispose();
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine(ex.ToString());

        //    return;
        //}

        //try
        //{
        //    DatabaseTargetBinaryBuilder targetBuilder =
        //        new DatabaseTargetBinaryBuilder();

        //    if (!targetBuilder.Exists)
        //    {
        //        Console.WriteLine("Please wait, building target binary database...");
        //        targetBuilder.Build();
        //    }

        //    targetBuilder.Dispose();
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine(ex.ToString());

        //    return;
        //}

        //try
        //{
        //    ReflectionIndexedBuilder indexedBuilder =
        //        new ReflectionIndexedBuilder();

        //    if (!indexedBuilder.Exists)
        //    {
        //        Console.WriteLine("Please wait, building reflection database...");
        //        indexedBuilder.AddDocuments();
        //    }

        //    indexedBuilder.Dispose();
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine(ex.ToString());

        //    return;
        //}


        //IList<BuildSpecialSdk> specialSdks = BuildSpecialSdks.InstalledBlendSilverlightSdks;
        //for (int i = 0; i < specialSdks.Count; i++)
        //{
        //    BuildSpecialSdk specialSdk = specialSdks[i];

        //    Console.WriteLine("\t{0}: {1} - {2}", i, specialSdk.Version,
        //        specialSdk.SdkType);
        //}
        //Console.WriteLine();
        //// Latest...
        //Console.WriteLine("Latest SDKs...");
        //BuildSpecialSdk latestSdk = BuildSpecialSdks.LatestWebMvcSdk;

        //Console.WriteLine("\t{0}: {1} - {2}", latestSdk.IsValid, latestSdk.Version,
        //    latestSdk.SdkType);

        //latestSdk = BuildSpecialSdks.LatestBlendWpfSdk;

        //Console.WriteLine("\t{0}: {1} - {2}", latestSdk.IsValid, latestSdk.Version,
        //    latestSdk.SdkType);

        //latestSdk = BuildSpecialSdks.LatestBlendSilverlightSdk;

        //Console.WriteLine("\t{0}: {1} - {2}", latestSdk.IsValid, latestSdk.Version,
        //    latestSdk.SdkType);
    }
}
