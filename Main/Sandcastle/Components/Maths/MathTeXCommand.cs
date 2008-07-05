using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Components.Maths
{
    /// <summary>
    ///  \newcommand{cmd}[args]{def}
    ///  \renewcommand{cmd}[args]{def}
    /// \newcommand{cmd}[args][opt]{def}
    ///  These commands define (or redefine) a command. 
    /// </summary>
    [Serializable]
    public sealed class MathTeXCommand : ICloneable
    {
        #region Private Fields

        private string _name;
        private string _command;
        private string _arguments;

        #endregion

        #region Constructors and Destructor

        public MathTeXCommand()
        {
        }

        public MathTeXCommand(string name, string command, string argument)
        {
            _name      = name;
            _command   = command;
            _arguments = argument;
        }

        public MathTeXCommand(MathTeXCommand source)
        {
            _name     = source._name;
            _command  = source._command;
            _arguments = source._arguments;
        }

        #endregion

        #region Public Properties

        public bool IsValid
        {
            get
            {
                if (String.IsNullOrEmpty(_name) || String.IsNullOrEmpty(_command))
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// A command name beginning with a \. For \newcommand it must not be already defined and must not begin with \end; for \renewcommand it must already be defined. 
        /// </summary>
        public string Name
        {
            get 
            { 
                return _name; 
            }

            set 
            { 
                _name = value; 
            }
        }

        /// <summary>
        /// The text to be substituted for every occurrence of cmd; a parameter of the form #n in cmd is replaced by the text of the nth argument when this substitution takes place. 
        /// </summary>
        public string Command
        {
            get 
            { 
                return _command; 
            }
            set 
            { 
                _command = value; 
            }
        }

        /// <summary>
        /// An integer from 1 to 9 denoting the number of arguments of the command being defined. The default is for the command to have no arguments.
        /// </summary>
        public string Arguments
        {
            get 
            { 
                return _arguments; 
            }
            set 
            { 
                _arguments = value; 
            }
        }

        #endregion

        #region Public Methods

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if (this.IsValid)
            {
                builder.Append("\\newcommand{");
                builder.Append(_name);
                builder.Append("}");
                if (String.IsNullOrEmpty(_arguments) == false)
                {
                    _arguments = _arguments.Trim();
                    string[] arguments = _arguments.Split(new char[] { ' ' });
                    for (int i = 0; i < arguments.Length; i++)
                    {
                        string argument = arguments[i].Trim();
                        if (String.IsNullOrEmpty(argument) == false)
                        {
                            builder.AppendFormat("[{0}]", argument);
                        }
                    }
                }
                builder.Append("{");
                builder.Append(_command);
                builder.Append("}");
            }

            return builder.ToString();
        }

        public void ToString(StringBuilder builder)
        {
            if (builder == null)
            {
                return;
            }

            if (this.IsValid)
            {
                builder.Append("\\newcommand{");
                builder.Append(_name);
                builder.Append("}");
                if (String.IsNullOrEmpty(_arguments) == false)
                {
                    _arguments = _arguments.Trim();
                    string[] arguments = _arguments.Split(new char[] { ' ' });
                    for (int i = 0; i < arguments.Length; i++)
                    {
                        string argument = arguments[i].Trim();
                        if (String.IsNullOrEmpty(argument) == false)
                        {
                            builder.AppendFormat("[{0}]", argument);
                        }
                    }
                }
                builder.Append("{");
                builder.Append(_command);
                builder.AppendLine("}");
            }
        }

        #endregion

        #region ICloneable Members

        public MathTeXCommand Clone()
        {
            MathTeXCommand package = new MathTeXCommand(this);

            if (_name != null)
            {
                package._name = String.Copy(_name);
            }
            if (_command != null)
            {
                package._command = String.Copy(_command);
            }
            if (_arguments != null)
            {
                package._arguments = String.Copy(_arguments);
            }

            return package;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
