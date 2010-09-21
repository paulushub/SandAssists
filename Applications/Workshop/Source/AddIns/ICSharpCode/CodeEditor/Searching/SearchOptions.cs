// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2650 $</version>
// </file>

using System;
using System.Windows.Forms;
using System.Collections.Generic;

using ICSharpCode.Core;

namespace ICSharpCode.TextEditor.Searching
{
	public static class SearchOptions
    {
        #region Public Constants

        public const int MaxFindCount          = 20;
        public const int MaxReplaceCount       = 20;
        public const int MaxDirectoryCount     = 20;
        public const int MaxUserDirectoryCount = 10;

        #endregion

        #region Private Fields

        private const string searchPropertyKey = "TextEditor.SearchProperties";
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
                properties.Set("LookInFiletypes", new StringList(patterns));
            }
            else
            {
                //string[] curPatterns = properties.Get(
                //    "LookInFiletypes", "").Split('\xFF'); ;
                StringList listPatterns = properties.Get("LookInFiletypes",
                    new StringList());

                if (listPatterns == null || listPatterns.Count <= 1)
                {
                    //properties.Set("LookInFiletypes", String.Join("\xFF", patterns));
                    properties.Set("LookInFiletypes", new StringList(patterns));
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
                    IList<string> oldPatterns = SearchOptions.FindPatterns;
                    int itemCount = (oldPatterns == null) ? 0 : oldPatterns.Count;
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
                        List<string> newPatterns = new List<string>(itemCount + 1);
                        // If the limit is not exceeded, we copy as usual...
                        newPatterns.Add(value);
                        if (oldPatterns != null && oldPatterns.Count != 0)
                        {
                            newPatterns.AddRange(oldPatterns);
                            // Otherwise, we discard last member(s)...
                            if (itemCount == SearchOptions.MaxFindCount)
                            {
                                newPatterns.RemoveAt(newPatterns.Count - 1);
                            }
                            else if (itemCount > SearchOptions.MaxFindCount)
                            {
                                while (itemCount >= SearchOptions.MaxFindCount)
                                {
                                    newPatterns.RemoveAt(newPatterns.Count - 1);
                                    itemCount = newPatterns.Count;
                                }
                            }
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
		
		public static IList<string> FindPatterns 
        {
			get 
            {
				if (!properties.Contains("FindPatterns")) 
                {
                    return new StringList();
				}

                StringList listPatterns = properties.Get("FindPatterns",
                    new StringList());
                //object findItems = properties.Get("FindPatterns");
                //string findPatterns = findItems as string;
                //if (findPatterns != null)
                //{
                //    StringList listItems = null;
                //    if (findPatterns.Length == 0)
                //    {
                //        listItems = new StringList();
                //    }
                //    else
                //    {
                //        listItems = new StringList(findPatterns.Split('\xFF'));
                //    }

                //    // Remove this old format...
                //    properties.Remove("FindPatterns");
                //    properties.Set("FindPatterns", listItems);

                //    return listItems;
                //}

                //IList<string> listPatterns = findItems as IList<string>;
                if (listPatterns != null)
                {
                    return listPatterns;
                }

                return new StringList();
			}
			set 
            {
                if (value != null && value.Count != 0)
                {
                    properties.Set("FindPatterns", new StringList(value));
                }
                else
                {
                    properties.Set("FindPatterns", new StringList());
                }
			}
		}
		
		public static string ReplacePattern 
        {
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
                    IList<string> oldPatterns = SearchOptions.ReplacePatterns;
                    int itemCount = (oldPatterns == null) ? 0 : oldPatterns.Count;
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
                        //string[] newPatterns = null;
                        //// If the limit is not exceeded, we copy as usual...
                        //if (itemCount < SearchOptions.MaxReplaceCount)
                        //{
                        //    newPatterns = new string[itemCount + 1];
                        //    newPatterns[0] = value;
                        //    oldPatterns.CopyTo(newPatterns, 1);
                        //}
                        //else // Otherwise, we discard last member(s)...
                        //{
                        //    itemCount = SearchOptions.MaxReplaceCount;

                        //    newPatterns = new string[itemCount];
                        //    newPatterns[0] = value;
                        //    Array.Copy(oldPatterns, 0, newPatterns, 1, itemCount - 1);
                        //}
                        List<string> newPatterns = new List<string>(itemCount + 1);
                        // If the limit is not exceeded, we copy as usual...
                        newPatterns.Add(value);
                        if (oldPatterns != null && oldPatterns.Count != 0)
                        {
                            newPatterns.AddRange(oldPatterns);
                            // Otherwise, we discard last member(s)...
                            if (itemCount == SearchOptions.MaxReplaceCount)
                            {
                                newPatterns.RemoveAt(newPatterns.Count - 1);
                            }
                            else if (itemCount > SearchOptions.MaxReplaceCount)
                            {
                                while (itemCount >= SearchOptions.MaxReplaceCount)
                                {
                                    newPatterns.RemoveAt(newPatterns.Count - 1);
                                    itemCount = newPatterns.Count;
                                }
                            }
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

        public static IList<string> ReplacePatterns 
        {
            get
            {
                if (!properties.Contains("ReplacePatterns"))
                {
                    return new StringList();
                }

                //string replacePatterns = properties.Get("ReplacePatterns", String.Empty);
                //if (String.IsNullOrEmpty(replacePatterns))
                //{
                //    return new StringList();
                //}

                StringList listPatterns = properties.Get("ReplacePatterns",
                    new StringList());
                //object replaceItems = properties.Get("ReplacePatterns");
                //string replacePatterns = replaceItems as string;
                //if (replacePatterns != null)
                //{
                //    StringList listItems = null;
                //    if (replacePatterns.Length == 0)
                //    {
                //        listItems = new StringList();
                //    }
                //    else
                //    {
                //        listItems = new StringList(replacePatterns.Split('\xFF'));
                //    }

                //    // Remove this old format...
                //    properties.Remove("ReplacePatterns");
                //    properties.Set("ReplacePatterns", listItems);

                //    return listItems;
                //}

                //IList<string> listPatterns = replaceItems as IList<string>;
                if (listPatterns != null)
                {
                    return listPatterns;
                }

                return new StringList();

                //return replacePatterns.Split('\xFF');
            }
            set
            {
                if (value != null && value.Count != 0)
                {
                    properties.Set("ReplacePatterns", new StringList(value));
                }
                else
                {
                    properties.Set("ReplacePatterns", new StringList());
                }
            }
		}
		
		public static bool MatchCase 
        {
			get 
            {
				return properties.Get("MatchCase", false);
			}
			set 
            {
				properties.Set("MatchCase", value);
			}
		}
		
		public static bool IncludeSubdirectories 
        {
			get 
            {
				return properties.Get("IncludeSubdirectories", false);
			}
			set 
            {
				properties.Set("IncludeSubdirectories", value);
			}
		}
	
		public static bool MatchWholeWord 
        {
			get 
            {
				return properties.Get("MatchWholeWord", false);
			}
			set 
            {
				properties.Set("MatchWholeWord", value);
			}
		}		
		
		public static string LookIn 
        {
			get 
            {
				return properties.Get("LookIn", @"C:\");
			}
			set 
            {
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
                    IList<string> oldLookInDirs = SearchOptions.LookInDirs;
                    int itemCount = oldLookInDirs.Count;
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
                        //string[] newLookInDirs = null;
                        //// If the limit is not exceeded, we copy as usual...
                        //if (itemCount < SearchOptions.MaxDirectoryCount)
                        //{
                        //    newLookInDirs = new string[itemCount + 1];
                        //    newLookInDirs[0] = value;
                        //    oldLookInDirs.CopyTo(newLookInDirs, 1);
                        //}
                        //else // Otherwise, we discard last member(s)...
                        //{
                        //    itemCount = SearchOptions.MaxDirectoryCount;

                        //    newLookInDirs = new string[itemCount];
                        //    newLookInDirs[0] = value;
                        //    Array.Copy(oldLookInDirs, 0, newLookInDirs, 1, itemCount - 1);
                        //}
                        List<string> newLookInDirs = new List<string>(itemCount + 1);
                        // If the limit is not exceeded, we copy as usual...
                        newLookInDirs.Add(value);
                        if (oldLookInDirs != null && oldLookInDirs.Count != 0)
                        {
                            newLookInDirs.AddRange(oldLookInDirs);
                            // Otherwise, we discard last member(s)...
                            if (itemCount == SearchOptions.MaxDirectoryCount)
                            {
                                newLookInDirs.RemoveAt(newLookInDirs.Count - 1);
                            }
                            else if (itemCount > SearchOptions.MaxDirectoryCount)
                            {
                                while (itemCount >= SearchOptions.MaxDirectoryCount)
                                {
                                    newLookInDirs.RemoveAt(newLookInDirs.Count - 1);
                                    itemCount = newLookInDirs.Count;
                                }
                            }
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

        public static IList<string> LookInDirs
        {
            get
            {
                if (!properties.Contains("LookInDirs"))
                {
                    return new StringList();
                }

                //string lookInDirs = properties.Get("LookInDirs", String.Empty);
                //if (String.IsNullOrEmpty(lookInDirs))
                //{
                //    return new StringList();
                //}

                //return lookInDirs.Split('\xFF');

                StringList listPatterns = properties.Get("LookInDirs",
                    new StringList());
                //object lookInItems = properties.Get("LookInDirs");
                //string lookInDirs = lookInItems as string;
                //if (lookInDirs != null)
                //{
                //    StringList listItems = null;
                //    if (lookInDirs.Length == 0)
                //    {
                //        listItems = new StringList();
                //    }
                //    else
                //    {
                //        listItems = new StringList(lookInDirs.Split('\xFF'));
                //    }

                //    // Remove this old format...
                //    properties.Remove("LookInDirs");
                //    properties.Set("LookInDirs", listItems);

                //    return listItems;
                //}

                //IList<string> listPatterns = lookInItems as IList<string>;
                if (listPatterns != null)
                {
                    return listPatterns;
                }

                return new StringList();
            }
            set
            {
                if (value != null && value.Count != 0)
                {
                    properties.Set("LookInDirs", new StringList(value));
                }
                else
                {
                    properties.Set("LookInDirs", new StringList());
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

        public static IList<string> LookInFiletypes
        {
            get
            {
                if (!properties.Contains("LookInFiletypes"))
                {
                    return new StringList();
                }

                //string lookInFiletypes = properties.Get("LookInFiletypes", String.Empty);
                //if (String.IsNullOrEmpty(lookInFiletypes))
                //{
                //    return new StringList();
                //}

                //return lookInFiletypes.Split('\xFF');

                StringList listPatterns = properties.Get("LookInFiletypes",
                    new StringList());
                //object lookInItems = properties.Get("LookInFiletypes");
                //string lookInFiletypes = lookInItems as string;
                //if (lookInFiletypes != null)
                //{
                //    StringList listItems = null;
                //    if (lookInFiletypes.Length == 0)
                //    {
                //        listItems = new StringList();
                //    }
                //    else
                //    {
                //        listItems = new StringList(lookInFiletypes.Split('\xFF'));
                //    }

                //    // Remove this old format...
                //    properties.Remove("LookInFiletypes");
                //    properties.Set("LookInFiletypes", listItems);

                //    return listItems;
                //}

                //IList<string> listPatterns = lookInItems as IList<string>;
                if (listPatterns != null)
                {
                    return listPatterns;
                }

                return new StringList();
            }
            set
            {
                if (value != null && value.Count != 0)
                {
                    properties.Set("LookInFiletypes", new StringList(value));
                }
                else
                {
                    properties.Set("LookInFiletypes", new StringList());
                }
            }
        }
		
		public static DocumentIteratorType DocumentIteratorType 
        {
			get 
            {
				return properties.Get("DocumentIteratorType", 
                    DocumentIteratorType.CurrentDocument);
			}
			set 
            {
				if (!Enum.IsDefined(typeof(DocumentIteratorType), value))
					throw new ArgumentException("invalid enum value");
				properties.Set("DocumentIteratorType", value);
			}
		}
		
		public static SearchStrategyType SearchStrategyType 
        {
			get 
            {
				return properties.Get("SearchStrategyType", 
                    SearchStrategyType.Normal);
			}
			set 
            {
				properties.Set("SearchStrategyType", value);
			}
		}

		#endregion
	}
}
