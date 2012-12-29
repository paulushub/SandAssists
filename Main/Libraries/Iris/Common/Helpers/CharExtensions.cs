using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Helpers {
	public static class CharExtensions {
		public static readonly string[] Low32CharNames =  {
				"NUL", "SOH", "STX", "ETX", "EOT", "ENQ", "ACK", "BEL", 
				"BS", "TAB", "LF", "VTAB", "FF", "CR", "SO", "SI",
				"DLE", "DC1", "DC2", "DC3", "DC4", "NAK", "SYN", "ETB",
				"CAN", "EM", "SUB", "ESC", "FS", "GS", "RS", "US", "SPACE"
			};
		
		public static string GetDescription(char c) {
			string printed;
			
			if (c <= 32) {
				printed = StringExtensions.Fi("<{0}>", Low32CharNames[c]);
			} else {
				printed = StringExtensions.Fi("'{0}' ", c);
			}

			return StringExtensions.Fi("{0} (U+{1:X4})", printed, (int) c);
		}
		
	}
}
