using System;

namespace ICSharpCode.SharpDevelop.Gui.ClassBrowser
{
	[Flags]
	public enum ClassBrowserFilter
	{
		None = 0,
		ShowProjectReferences = 1,
		ShowBaseAndDerivedTypes = 32,
		
		ShowPublic = 2,
		ShowProtected = 4,
		ShowPrivate = 8,
		ShowOther = 16,
		
		All = ShowProjectReferences | ShowPublic | ShowProtected | ShowPrivate | ShowOther | ShowBaseAndDerivedTypes
	}
}
