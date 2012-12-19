// Copyright © Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.CommandLine
{      
    public sealed class ParseArgumentsResult
    {     
        // data
        private OptionCollection _options;

        private Dictionary<string, ParseResult> _errors;

        private List<string> _unusedArguments;

        internal ParseArgumentsResult() 
        {
            _errors          = new Dictionary<string, ParseResult>();
            _unusedArguments = new List<string>();
        }

        // accessors

        public OptionCollection Options
        {
            get
            {
                return _options;
            }
            internal set
            {
                this._options = value;
            }
        }

        public bool Success
        {
            get
            {
                return (_errors.Count == 0);
            }
        }

        public IList<string> UnusedArguments
        {
            get
            {
                return this._unusedArguments;
            }
        }

        public IDictionary<string, ParseResult> Errors
        {
            get
            {
                return this._errors;
            }
        }

        public void WriteParseErrors(TextWriter writer)
        {     
            if (writer == null) 
                throw new ArgumentNullException("writer");

            foreach (KeyValuePair<string, ParseResult> error in _errors)
            {
                writer.WriteLine("{0}: {1}", error.Value, error.Key);
            }                                     
        }

        public static string[] SplitCommandLineArgument(String argumentString)
        {
            StringBuilder translatedArguments = new StringBuilder(argumentString).Replace("\\\"", "\r");
            bool InsideQuote = false;
            for (int i = 0; i < translatedArguments.Length; i++)
            {
                if (translatedArguments[i] == '"')
                {
                    InsideQuote = !InsideQuote;
                }
                if (translatedArguments[i] == ' ' && !InsideQuote)
                {
                    translatedArguments[i] = '\n';
                }
            }

            string[] toReturn = translatedArguments.ToString().Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < toReturn.Length; i++)
            {
                toReturn[i] = RemoveMatchingQuotes(toReturn[i]);
                toReturn[i] = toReturn[i].Replace("\r", "\"");
            }
            return toReturn;
        }

        private static string RemoveMatchingQuotes(string stringToTrim)
        {
            int firstQuoteIndex = stringToTrim.IndexOf('"');
            int lastQuoteIndex = stringToTrim.LastIndexOf('"');
            while (firstQuoteIndex != lastQuoteIndex)
            {
                stringToTrim = stringToTrim.Remove(firstQuoteIndex, 1);
                stringToTrim = stringToTrim.Remove(lastQuoteIndex - 1, 1); //-1 because we've shifted the indices left by one
                firstQuoteIndex = stringToTrim.IndexOf('"');
                lastQuoteIndex = stringToTrim.LastIndexOf('"');
            }
            return stringToTrim;
        }
    }       
}
