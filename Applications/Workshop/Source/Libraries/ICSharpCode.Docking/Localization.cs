using System;
using System.ComponentModel;

// To simplify the process of finding the toolbox bitmap resource:
// #1 Create an internal class called "resfinder" outside of the root namespace.
// #2 Use "resfinder" in the toolbox bitmap attribute instead of the control name.
// #3 use the "<default namespace>.<resourcename>" string to locate the resource.
// See: http://www.bobpowell.net/toolboxbitmap.htm
internal sealed class resfinder
{
}


namespace WeifenLuo.WinFormsUI.Docking
{
	[AttributeUsage(AttributeTargets.All)]
	internal sealed class LocalizedDescriptionAttribute : DescriptionAttribute
	{
		private bool m_initialized = false;

		public LocalizedDescriptionAttribute(string key) : base(key)
		{
		}

		public override string Description
		{
			get
			{	
				if (!m_initialized)
				{
					string key = base.Description;
					DescriptionValue = ResourceHelper.GetString(key);
					if (DescriptionValue == null)
						DescriptionValue = String.Empty;

					m_initialized = true;
				}

				return DescriptionValue;
			}
		}
	}

	[AttributeUsage(AttributeTargets.All)]
	internal sealed class LocalizedCategoryAttribute : CategoryAttribute
	{
		public LocalizedCategoryAttribute(string key) : base(key)
		{
		}

		protected override string GetLocalizedString(string key)
		{
			return ResourceHelper.GetString(key);
		}
	}
}
