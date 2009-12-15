// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2650 $</version>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.TextEditor.Searching
{
	public static class SearchOptions
    {
        #region Private Fields/Constants

        private const int MaxFindCount          = 20;
        private const int MaxReplaceCount       = 20;
        private const int MaxDirectoryCount     = 20;
        private const int MaxUserDirectoryCount = 10;

		private const string searchPropertyKey = "SearchAndReplaceProperties";
        private static string replacePattern = String.Empty;

        private static Properties properties;
        private static ToolStripRenderer stripRenderer;

        #endregion

        #region Static Constructor

        static SearchOptions()
        {
            properties = PropertyService.Get(searchPropertyKey, new Properties());

            // Our default list of file filter, mainly C#, VB.NET, ASP.NET/Web projects
            string[] patterns = new string[]
                {
                    "*.cs;*.resx;*.xsd;*.wsdl;*.xaml;*.xml;*.htm;*.html;*.css;*.addin",
                    "*.cs;*.resx;*.xsd;*.wsdl;*.xaml;*.xml;*.htm;*.html;*.css",
                    "*.vb;*.resx;*.xsd;*.wsdl;*.xaml;*.xml;*.htm;*.html;*.css",
                    "*.vb;*.resx;*.xsd;*.wsdl;*.htm;*.html;*.aspx;*.ascx;*.asmx;*.svc;*.asax;*.config;*.asp;*.asa;*.css;*.xml",
                    "*.vb;*.resx;*.xsd;*.wsdl;*.xml;*.htm;*.html;*.css",
                    "*.c;*.cpp;*.cxx;*.cc;*.tli;*.tlh;*.h;*.hpp;*.hxx;*.hh;*.inl;*.rc;*.resx;*.idl;*.asm;*.inc",
                    "*.cs;*.resx;*.xsd;*.wsdl;*.xml;*.htm;*.html;*.css",
                    "*.srf;*.htm;*.html;*.xml;*.gif;*.jpg;*.png;*.css;*.disco",
                    "*.txt",
                    "*.*"
                };
            if (!properties.Contains("LookInFiletypes"))
            {
                properties.Set("LookInFiletypes", String.Join("\xFF", patterns));
            }
            else
            {
                string[] curPatterns = properties.Get(
                    "LookInFiletypes", "").Split('\xFF'); ;
               
                if (curPatterns == null || curPatterns.Length <= 1)
                {
                    properties.Set("LookInFiletypes", String.Join("\xFF", patterns));
                }
            }
        }

        #endregion

        #region Public Properties

        public static Properties Properties 
        {
			get 
            {
				return properties;
			}
		}

        public static ToolStripRenderer StripRenderer
        {
            get
            {
                return stripRenderer;
            }
            set
            {
                if (value != null)
                {
                    stripRenderer = value;
                }
            }
        }

        public static string FindPattern 
        {
			get 
            {
                if (properties.Contains("FindPattern"))
                {
                    return properties["FindPattern"];
                }

                return String.Empty;
			}
			set 
            {
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }

                string curPattern = SearchOptions.FindPattern;
                if (!String.IsNullOrEmpty(value)) 
                {
                    string[] oldPatterns = SearchOptions.FindPatterns;
                    int itemCount = oldPatterns.Length;
                    bool notFound = true;
                    for (int i = 0; i < itemCount; i++)
                    {
                        if (String.Equals(oldPatterns[i], value, 
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            notFound = false;
                        }
                    }
                    if (notFound)
                    {
                        string[] newPatterns = null;
                        // If the limit is not exceeded, we copy as usual...
                        if (itemCount < SearchOptions.MaxFindCount)
                        {
                            newPatterns = new string[itemCount + 1];
                            newPatterns[0] = value;
                            oldPatterns.CopyTo(newPatterns, 1); 
                        }
                        else // Otherwise, we discard last member(s)...
                        {
                            itemCount = SearchOptions.MaxFindCount;

                            newPatterns = new string[itemCount];
                            newPatterns[0] = value;
                            Array.Copy(oldPatterns, 0, newPatterns, 1, itemCount - 1);
                        }

                        SearchOptions.FindPatterns = newPatterns;
                    }
				}

                if (!String.Equals(value, curPattern,
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    properties["FindPattern"] = value;
                }
			}
		}
		
		public static string[] FindPatterns 
        {
			get 
            {
				if (!properties.Contains("FindPatterns")) 
                {
					return new string[] {};
				}

                string findPatterns = properties.Get("FindPatterns", String.Empty);
                if (String.IsNullOrEmpty(findPatterns))
                {
                    return new string[] { };
                }

                return findPatterns.Split('\xFF');
			}
			set 
            {
                if (value != null && value.Length != 0)
                {
                    properties.Set("FindPatterns", String.Join("\xFF", value));
                }
                else
                {
                    properties.Set("FindPatterns", String.Empty);
                }
			}
		}
		
		public static string ReplacePattern 
        {
            //get 
            //{
            //    if (!properties.Contains("ReplacePatterns")) 
            //    {
            //        return String.Empty;
            //    }

            //    return replacePattern;
            //}
            //set 
            //{
            //    if (String.IsNullOrEmpty(value))
            //    {
            //        return;
            //    }

            //    if (value != ReplacePattern) 
            //    {
            //        string[] oldPatterns = ReplacePatterns;
            //        string[] newPatterns = new string[oldPatterns.Length + 1];
            //        oldPatterns.CopyTo(newPatterns, 1);
            //        newPatterns[0] = value;
            //        ReplacePatterns = newPatterns;
            //        replacePattern = value;
            //    }
            //}
            get
            {
                if (properties.Contains("ReplacePattern"))
                {
                    return properties["ReplacePattern"];
                }

                return String.Empty;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }

                string curPattern = SearchOptions.ReplacePattern;
                if (!String.IsNullOrEmpty(value))
                {
                    string[] oldPatterns = SearchOptions.ReplacePatterns;
                    int itemCount = oldPatterns.Length;
                    bool notFound = true;
                    for (int i = 0; i < itemCount; i++)
                    {
                        if (String.Equals(oldPatterns[i], value,
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            notFound = false;
                        }
                    }
                    if (notFound)
                    {
                        string[] newPatterns = null;
                        // If the limit is not exceeded, we copy as usual...
                        if (itemCount < SearchOptions.MaxReplaceCount)
                        {
                            newPatterns = new string[itemCount + 1];
                            newPatterns[0] = value;
                            oldPatterns.CopyTo(newPatterns, 1);
                        }
                        else // Otherwise, we discard last member(s)...
                        {
                            itemCount = SearchOptions.MaxReplaceCount;

                            newPatterns = new string[itemCount];
                            newPatterns[0] = value;
                            Array.Copy(oldPatterns, 0, newPatterns, 1, itemCount - 1);
                        }

                        SearchOptions.ReplacePatterns = newPatterns;
                    }
                }

                if (!String.Equals(value, curPattern,
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    properties["ReplacePattern"] = value;
                }
            }
        }
		
		public static string[] ReplacePatterns 
        {
            get
            {
                if (!properties.Contains("ReplacePatterns"))
                {
                    return new string[] { };
                }

                string replacePatterns = properties.Get("ReplacePatterns", String.Empty);
                if (String.IsNullOrEmpty(replacePatterns))
                {
                    return new string[] { };
                }

                return replacePatterns.Split('\xFF');
            }
            set
            {
                if (value != null && value.Length != 0)
                {
                    properties.Set("ReplacePatterns", String.Join("\xFF", value));
                }
                else
                {
                    properties.Set("ReplacePatterns", String.Empty);
                }
            }
		}
		
		public static bool MatchCase {
			get {
				return properties.Get("MatchCase", false);
			}
			set {
				properties.Set("MatchCase", value);
			}
		}
		
		public static bool IncludeSubdirectories {
			get {
				return properties.Get("IncludeSubdirectories", false);
			}
			set {
				properties.Set("IncludeSubdirectories", value);
			}
		}
	
		public static bool MatchWholeWord {
			get {
				return properties.Get("MatchWholeWord", false);
			}
			set {
				properties.Set("MatchWholeWord", value);
			}
		}		
		
		public static string LookIn {
			get {
				return properties.Get("LookIn", @"C:\");
			}
			set {
				properties.Set("LookIn", value);
			}
		}
		
		public static string LookInDir 
        {
            get
            {
                if (properties.Contains("LookInDir"))
                {
                    return properties["LookInDir"];
                }

                return String.Empty;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    properties["LookInDir"] = String.Empty;
                    return;
                }

                string curLookInDir = SearchOptions.LookInDir;
                if (!String.IsNullOrEmpty(value))
                {
                    string[] oldLookInDirs = SearchOptions.LookInDirs;
                    int itemCount = oldLookInDirs.Length;
                    bool notFound = true;
                    for (int i = 0; i < itemCount; i++)
                    {
                        if (String.Equals(oldLookInDirs[i], value,
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            notFound = false;
                        }
                    }
                    if (notFound)
                    {
                        string[] newLookInDirs = null;
                        // If the limit is not exceeded, we copy as usual...
                        if (itemCount < SearchOptions.MaxDirectoryCount)
                        {
                            newLookInDirs = new string[itemCount + 1];
                            newLookInDirs[0] = value;
                            oldLookInDirs.CopyTo(newLookInDirs, 1);
                        }
                        else // Otherwise, we discard last member(s)...
                        {
                            itemCount = SearchOptions.MaxDirectoryCount;

                            newLookInDirs = new string[itemCount];
                            newLookInDirs[0] = value;
                            Array.Copy(oldLookInDirs, 0, newLookInDirs, 1, itemCount - 1);
                        }

                        SearchOptions.LookInDirs = newLookInDirs;
                    }
                }

                if (!String.Equals(value, curLookInDir,
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    properties["LookInDir"] = value;
                }
            }
        }

        public static string[] LookInDirs
        {
            get
            {
                if (!properties.Contains("LookInDirs"))
                {
                    return new string[] { };
                }

                string lookInDirs = properties.Get("LookInDirs", String.Empty);
                if (String.IsNullOrEmpty(lookInDirs))
                {
                    return new string[] { };
                }

                return lookInDirs.Split('\xFF');
            }
            set
            {
                if (value != null && value.Length != 0)
                {
                    properties.Set("LookInDirs", String.Join("\xFF", value));
                }
                else
                {
                    properties.Set("LookInDirs", String.Empty);
                }
            }
        }

        public static string LookInFiletype
        {
            get
            {
                return properties.Get("LookInFiletype", "*.*");
            }
            set
            {
                properties.Set("LookInFiletype", value);
            }
        }

        public static string[] LookInFiletypes
        {
            get
            {
                if (!properties.Contains("LookInFiletypes"))
                {
                    return new string[] { };
                }
                return properties.Get("LookInFiletypes", "").Split('\xFF');
            }
            set
            {
                properties.Set("LookInFiletypes", String.Join("\xFF", value));
            }
        }
		
		public static DocumentIteratorType DocumentIteratorType {
			get {
				return properties.Get("DocumentIteratorType", DocumentIteratorType.CurrentDocument);
			}
			set {
				if (!Enum.IsDefined(typeof(DocumentIteratorType), value))
					throw new ArgumentException("invalid enum value");
				properties.Set("DocumentIteratorType", value);
			}
		}
		
		public static SearchStrategyType SearchStrategyType {
			get {
				return properties.Get("SearchStrategyType", SearchStrategyType.Normal);
			}
			set {
				properties.Set("SearchStrategyType", value);
			}
		}

		#endregion
	}
}
