//
// THIS FILE IS OVERWRITTEN BY THE BUILD PROCESS (but never committed). If you want to change
// its permanent contents, please edit the Iris.proj build file.
//

using System.Reflection;

// Our assemblies are versioned thus:
//
//	  Major and Minor Versions are hard coded (currently 0.1)
//
//	  The next number (which .NET calls the "build number") is YYMM (eg, 0709 for September, 2007)
//	  Then we have DDbb, which is the day followed the build number in that day (eg, 2902 for the second
//	  build on the 29th). The full version number x.y.YYMM.DDbb, which in all its glory turns out to be:
//	  x.y.0709.2902, which is the second build on the 29th of September in the 2007th year of our Lord
//		
//	  The build label is YYYY.mm.dd-bb, which is slightly more human friendly. In our source control, 
//	  there is a tag for each build. If you want to retrieve the exact snapshot of the sources used in 
//	  the example build above, they will be in project/tags/2007.09.29-02
//		
//	  Have fun.
[assembly: AssemblyVersion("0.1.0.0")]
[assembly: AssemblyFileVersion("0.1.0.0")]
[assembly: AssemblyDescription("Build 2000.01.01-1 on xyz, 1 January 2000 at 00:00 UTC")]