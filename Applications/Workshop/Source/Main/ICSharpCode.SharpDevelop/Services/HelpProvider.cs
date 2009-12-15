// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 4944 $</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.SharpDevelop
{
	public class HelpProvider
	{
        private static HelpProvider _provider;

        public HelpProvider()
        {
        }

        public static HelpProvider Provider
        {
            get
            {
                if (_provider == null)
                {
                    _provider = new HelpProvider();
                }

                return _provider;
            }
        }

		public static List<HelpProvider> GetProviders()
		{
			return AddInTree.BuildItems<HelpProvider>(
                "/SharpDevelop/Services/HelpProvider", null, false);
		}
		
		public static void ShowHelp(IClass c)
		{
            if (c == null)
                throw new ArgumentNullException("c");

            foreach (HelpProvider p in GetProviders())
            {
				if (p.TryShowHelp(c))
					return;
			}

            Provider.TryShowHelp(c);
		}
		
		public virtual bool TryShowHelp(IClass c)
		{
            if (c == null)
                throw new ArgumentNullException("c");

            return TryShowHelp(c.FullyQualifiedName);
		}
		
		public static void ShowHelp(IMember m)
		{
            if (m == null)
                throw new ArgumentNullException("m");

            foreach (HelpProvider p in GetProviders())
            {
				if (p.TryShowHelp(m))
					return;
			}

            Provider.TryShowHelp(m);
		}
		
		public virtual bool TryShowHelp(IMember m)
		{
            if (m == null)
                throw new ArgumentNullException("m");

            IMethod method = m as IMethod;
            if (method != null && method.IsConstructor)
                return TryShowHelp(m.DeclaringType.FullyQualifiedName + "." + m.DeclaringType.Name);
            else
                return TryShowHelp(m.FullyQualifiedName);
		}
		
		public static void ShowHelp(string fullTypeName)
		{
            if (fullTypeName == null)
                throw new ArgumentNullException("fullTypeName");

            foreach (HelpProvider p in GetProviders())
            {
				if (p.TryShowHelp(fullTypeName))
					return;
			}

            Provider.TryShowHelp(fullTypeName);
		}
		
		public virtual bool TryShowHelp(string fullTypeName)
		{
            if (fullTypeName == null)
                throw new ArgumentNullException("fullTypeName");

            FileService.OpenFile("http://msdn2.microsoft.com/library/" + 
                Uri.EscapeDataString(fullTypeName));

			return true;
		}
		
		public static void ShowHelpByKeyword(string keyword)
		{
            if (keyword == null)
                throw new ArgumentNullException("keyword");
            foreach (HelpProvider p in GetProviders())
            {
				if (p.TryShowHelpByKeyword(keyword))
					return;
			}
		}
		
		public virtual bool TryShowHelpByKeyword(string keyword)
		{
			return false;
		}
	}
}