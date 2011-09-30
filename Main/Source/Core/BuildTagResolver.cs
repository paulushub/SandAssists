using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Sandcastle
{
    public abstract class BuildTagResolver : BuildObject
    {
        #region Private Fields

        private static BuildTagResolver _defaultResolver;

        private bool _isResolving;

        private BuildDictionary<Func<string, string>>   _userFunctions;
        private BuildMultiMap<string, BuildTagProvider> _userProviders;

        #endregion

        #region Constructors and Destructor

        protected BuildTagResolver()
        {
            _userProviders = new BuildMultiMap<string, BuildTagProvider>(
                StringComparer.OrdinalIgnoreCase);
            _userFunctions = new BuildDictionary<Func<string, string>>();
        }

        #endregion

        #region Public Properties

        public virtual bool IsResolving
        {
            get
            {
                return _isResolving;
            }
        }

        public static BuildTagResolver Resolver
        {
            get
            {
                if (_defaultResolver == null)
                {
                    _defaultResolver = BuildTagResolver.Create();
                }

                return _defaultResolver;
            }
        }

        #endregion

        #region Protected Properties

        protected BuildMultiMap<string, BuildTagProvider> Providers
        {
            get
            {
                return _userProviders;
            }
        }

        protected BuildDictionary<Func<string, string>> Functions
        {
            get
            {
                return _userFunctions;
            }
        }

        #endregion

        #region Public Methods

        public static BuildTagResolver Create()
        {
            return new SystemTagResolver();
        }

        public static string ResolveText(string sourceText)
        {
            return BuildTagResolver.ResolveText(sourceText, null);
        }

        public static string ResolveText(string sourceText, BuildTagProvider provider)
        {
            BuildTagResolver resolver = BuildTagResolver.Resolver;

            return resolver.Resolve(sourceText, provider);
        }

        public virtual void BeginResolve()
        {
            if (_isResolving)
            {
                return;
            }

            _isResolving = true;
        }

        public virtual void EndResolve()
        {
            _isResolving = false;
        }

        public virtual string Resolve(string sourceText)
        {
            return this.Resolve(sourceText, null);
        }

        public virtual string Resolve(string sourceText, BuildTagProvider provider)
        {
            // Is the resolving mode started by the user or by this method?
            bool startedResolving = false;

            if (!_isResolving)
            {
                this.BeginResolve();
                startedResolving = true;
            }

            // Process the tag...
            string resolvedText = this.OnResolve(sourceText, provider);
            if (String.IsNullOrEmpty(resolvedText))
            {
                resolvedText = sourceText;
            }

            if (startedResolving)
            {
                this.EndResolve();
            }

            return resolvedText;
        }

        public virtual bool RegisterProvider(BuildTagProvider provider)
        {
            BuildExceptions.NotNull(provider, "provider");

            if (String.IsNullOrEmpty(provider.Id) || String.IsNullOrEmpty(provider.Category))
            {
                return false;
            }

            _userProviders.Add(provider.Category, provider);

            return true;
        }
        
        public virtual bool UnregisterProvider(BuildTagProvider provider)
        {
            BuildExceptions.NotNull(provider, "provider");

            return this.UnregisterProvider(provider.Category, provider.Id);
        }                      
                                 
        public virtual bool UnregisterProvider(string providerCategory, string providerId)
        {
            if (String.IsNullOrEmpty(providerId) || String.IsNullOrEmpty(providerCategory))
            {
                return false;
            }

            IList<BuildTagProvider> providers = _userProviders[providerCategory];
            if (providers != null && providers.Count != 0)
            {
                int foundAt = -1;
                for (int i = 0; i < providers.Count; i++)
                {
                    if (String.Equals(providerId, providers[i].Id, StringComparison.OrdinalIgnoreCase))
                    {
                        foundAt = i;
                        break;
                    }
                }

                if (foundAt >= 0)
                {
                    providers.RemoveAt(foundAt);

                    return true;
                }
            }

            return false;
        }

        public virtual bool RegisterFunction(string functionName, 
            Func<string, string> function)
        {
            BuildExceptions.NotNullNotEmpty(functionName, "functionName");
            BuildExceptions.NotNull(function, "function");

            _userFunctions[functionName] = function;

            return true;
        }

        public virtual bool UnregisterFunction(string functionName)
        {
            BuildExceptions.NotNullNotEmpty(functionName, "functionName");

            return _userFunctions.Remove(functionName);
        }

        #endregion

        #region Protected Methods

        protected virtual Func<string, string> GetFunction(string functionName)
        {
            if (String.IsNullOrEmpty(functionName))
            {
                return null;
            }

            return _userFunctions[functionName];
        }

        protected virtual BuildTagProvider GetProvider(string tagName)
        {
            return this.GetProvider(BuildTagProvider.NoCategory, tagName);
        }

        protected virtual BuildTagProvider GetProvider(string category, string tagName)
        {
            if (String.IsNullOrEmpty(category) || String.IsNullOrEmpty(tagName))
            {
                return null;
            }

            BuildTagProvider provider = null;
            IList<BuildTagProvider> providers = _userProviders[category];
            if (providers != null && providers.Count != 0)
            {
                for (int i = 0; i < providers.Count; i++)
                {
                    BuildTagProvider tagProvider = providers[i];
                    if (tagProvider.IsSupported(tagName))
                    {
                        provider = tagProvider;
                        break;
                    }
                }  
            }

            return provider;
        }

        protected abstract string OnResolve(string sourceText, BuildTagProvider provider);

        #endregion

        #region SystemTagResolver Class

        private sealed class SystemTagResolver : BuildTagResolver
        {
            #region Private Fields

            private Regex  _regEx; 
            private Guid[] _guidValues;

            private BuildDictionary<Func<string, string>>   _functions;
            private BuildMultiMap<string, BuildTagProvider> _providers;

            #endregion

            #region Constructors and Destructor

            public SystemTagResolver()
            {
                _regEx = new Regex(@"\$\{(?<TagName>[^\$\{\}]*)\}", RegexOptions.Compiled);

                _providers = new BuildMultiMap<string, BuildTagProvider>(
                    StringComparer.OrdinalIgnoreCase);
                _functions = new BuildDictionary<Func<string, string>>();

                EnvironmentTagProvider envProvider = new EnvironmentTagProvider();
                _providers.Add(envProvider.Category, envProvider);

                NoCategoryTagProvider noCatProvider = new NoCategoryTagProvider();
                _providers.Add(noCatProvider.Category, noCatProvider);

                // Add some functions...
                _functions.Add("String.ToLower", new Func<string, string>(ToLower));
                _functions.Add("String.ToUpper", new Func<string, string>(ToUpper));

                _guidValues = new Guid[10];
                for (int i = 0; i < 10; i++)
                {
                    _guidValues[i] = Guid.NewGuid();
                }
            }

            #endregion

            #region Public Methods

            public override void BeginResolve()
            {
                base.BeginResolve();

                // We create new Guid values for this resolution...
                for (int i = 0; i < 10; i++)
                {
                    _guidValues[i] = Guid.NewGuid();
                }
            }

            public override void EndResolve()
            {
                base.EndResolve();
            }

            #endregion

            #region Protected Methods

            protected override string OnResolve(string sourceText, BuildTagProvider provider)
            {              
                StringBuilder inputText = new StringBuilder(sourceText);

                Match match = _regEx.Match(inputText.ToString());
                while (match != null && match.Success)
                {
                    int index    = match.Index;
                    int length   = match.Length;
                    string value = match.Value;

                    string name = match.Groups["TagName"].Value;

                    string[] parts      = name.Split('|');
                    string namePart     = String.Empty;
                    string formatPart   = String.Empty;
                    string functionPart = String.Empty;

                    switch (parts.Length)
                    {
                        case 1:
                            namePart = parts[0];
                            break;
                        case 2:
                            namePart = parts[0];
                            string tempText = parts[1].Trim();
                            if (tempText.StartsWith("(", StringComparison.Ordinal) &&
                                tempText.EndsWith(")", StringComparison.Ordinal))
                            {
                                functionPart = parts[1].Substring(1, parts[1].Length - 2);
                            }
                            else
                            {
                                formatPart = parts[1];
                            }
                            break;
                        case 3:
                            namePart     = parts[0];
                            formatPart   = parts[1];
                            functionPart = parts[2].Substring(1, parts[2].Length - 2);
                            break;
                        default:
                            throw new ArgumentException();
                    }

                    string categoryPart = String.Empty;
                    string numberPart = String.Empty;

                    int foundAt = namePart.IndexOf(':');
                    if (foundAt > 0)
                    {
                        categoryPart = namePart.Substring(0, foundAt).Trim();
                        namePart = namePart.Substring(foundAt + 1).Trim();
                    }
                    foundAt = namePart.IndexOf('[');
                    if (foundAt > 0 && namePart.EndsWith("]", StringComparison.Ordinal))
                    {
                        numberPart = namePart.Substring(foundAt + 1, namePart.Length - foundAt - 2).Trim();
                        namePart = namePart.Substring(0, foundAt).Trim();
                    }

                    string replacement = this.GetReplacement(namePart, categoryPart,
                        numberPart, formatPart, functionPart, provider);
                    if (!String.IsNullOrEmpty(replacement))
                    {
                        // If a replacement is found, apply it...
                        inputText.Remove(index, length);      // delete the current
                        inputText.Insert(index, replacement); // insert the new
                    }

                    match = _regEx.Match(inputText.ToString(), index);
                }

                return inputText.ToString();
            }

            protected override Func<string, string> GetFunction(string functionName)
            {
                // Give priority to the default registered functions...
                Func<string, string> function = base.GetFunction(functionName);

                if (function != null)
                {
                    return function;
                }

                return _functions[functionName];
            }

            protected override BuildTagProvider GetProvider(string category, string tagName)
            {
                // Give priority to the default registered providers...
                BuildTagProvider provider = base.GetProvider(category, tagName);
                if (provider != null)
                {
                    return provider;
                }

                IList<BuildTagProvider> providers = _providers[category];
                if (providers != null && providers.Count != 0)
                {
                    for (int i = 0; i < providers.Count; i++)
                    {
                        BuildTagProvider tagProvider = providers[i];
                        if (tagProvider.IsSupported(tagName))
                        {
                            provider = tagProvider;
                            break;
                        }
                    }
                }

                return provider;
            }

            #endregion

            #region Private Methods

            private string GetReplacement(string tagName, string category, string numberText, 
                string formatText, string functionName, BuildTagProvider provider)
            {
                string replacement = null;

                if (provider != null && String.Equals(provider.Category, category) &&
                    provider.IsSupported(tagName))
                {
                    replacement = provider.GetText(tagName, numberText, formatText);
                }
                else
                {
                    BuildTagProvider tagProvider = this.GetProvider(category, tagName);
                    if (tagProvider != null)
                    {
                        replacement = tagProvider.GetText(tagName, numberText, formatText);
                    }
                    else
                    {
                        // We have the default processing...
                        replacement = this.GetReplacement(tagName, numberText, formatText);
                    }
                }

                if (!String.IsNullOrEmpty(replacement) && !String.IsNullOrEmpty(functionName))
                {
                    Func<string, string> function = this.GetFunction(functionName);

                    if (function != null)
                    {
                        replacement = function(replacement);
                    }
                }

                return replacement;
            }

            private string GetReplacement(string tagName, string numberText, string formatText)
            {
                if (tagName.Equals("date", StringComparison.OrdinalIgnoreCase))
                {
                    return DateTime.Today.ToShortDateString();
                }
                
                if (tagName.Equals("time", StringComparison.OrdinalIgnoreCase))
                {
                    return DateTime.Now.ToShortTimeString();
                }

                if (tagName.Equals("datetime", StringComparison.OrdinalIgnoreCase))
                {
                    DateTime value = DateTime.Now;
                    if (!String.IsNullOrEmpty(formatText))
                    {
                        return value.ToString(formatText);
                    }

                    return value.ToString();
                }
                
                if (tagName.Equals("guid", StringComparison.OrdinalIgnoreCase))
                {
                    if (!String.IsNullOrEmpty(numberText))
                    {
                        int numValue;
                        if (Int32.TryParse(numberText, out numValue) && 
                            (numValue >= 0 && numValue <= 9))
                        {
                            Guid indexedValue = _guidValues[numValue];
                            if (!String.IsNullOrEmpty(formatText))
                            {
                                return indexedValue.ToString(formatText);
                            }

                            return indexedValue.ToString();
                        }
                    }

                    Guid value = Guid.NewGuid();
                    if (!String.IsNullOrEmpty(formatText))
                    {
                        return value.ToString(formatText);
                    }

                    return value.ToString();
                }

                return null;
            }

            #endregion

            #region Private Static Functions

            private static string ToLower(string inputText)
            {
                if (String.IsNullOrEmpty(inputText))
                {
                    return String.Empty;
                }

                return inputText.ToLower();
            }

            private static string ToUpper(string inputText)
            {
                if (String.IsNullOrEmpty(inputText))
                {
                    return String.Empty;
                }

                return inputText.ToUpper();
            }

            #endregion

            #region EnvironmentTagProvider Class

            private sealed class EnvironmentTagProvider : BuildTagProvider
            {
                #region Private Fields

                private BuildDictionary<string> _variables;

                #endregion

                #region Constructors and Destructor

                public EnvironmentTagProvider()                   
                {
                    _variables = new BuildDictionary<string>();

                    foreach (System.Collections.DictionaryEntry de in 
                        Environment.GetEnvironmentVariables())
                    {
                        _variables[(string)de.Key] = (string)de.Value;
                    }
                }

                #endregion

                #region Public Properties

                public override string Category
                {
                    get
                    {
                        return "Env";
                    }
                }

                #endregion

                #region Public Methods

                public override bool IsSupported(string tagName)
                {
                    return _variables.ContainsKey(tagName);
                }

                public override string GetText(string tagName, string numberText, string formatText)
                {
                    // The environment variables do not offer support for formatting...
                    return _variables[tagName];
                }

                #endregion
            }

            #endregion 

            #region NoCategoryTagProvider Class

            private sealed class NoCategoryTagProvider : BuildTagProvider
            {
                #region Private Fields

                private BuildDictionary<string> _variables;

                #endregion

                #region Constructors and Destructor

                public NoCategoryTagProvider()                   
                {
                    _variables = new BuildDictionary<string>();

                    _variables["User"]           = Environment.UserName;
                    _variables["UserName"]       = Environment.UserName;

                    _variables["Machine"]        = Environment.MachineName;
                    _variables["MachineName"]    = Environment.MachineName;
                    _variables["UserDomain"]     = Environment.UserDomainName;
                    _variables["UserDomainName"] = Environment.UserDomainName;

                    if (IntPtr.Size == 4)
                    {
                        _variables["Platform"] = "Win32";
                    }
                    else if (IntPtr.Size == 8)
                    {
                        _variables["Platform"] = "Win64";
                    }
                    else
                    {
                        _variables["Platform"] = "Unknown";
                    }
                }

                #endregion

                #region Public Properties

                #endregion

                #region Public Methods

                public override bool IsSupported(string tagName)
                {
                    return _variables.ContainsKey(tagName);
                }

                public override string GetText(string tagName, string numberText, string formatText)
                {
                    // The environment variables do not offer support for formatting...
                    return _variables[tagName];
                }

                #endregion
            }

            #endregion 
        }

        #endregion
    }
}
