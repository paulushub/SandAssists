// Copyright © Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Ddue.Tools
{                
    public class BuildAssembler : IDisposable
    {
        #region Private Fields

        private Type                 _thisType;

        // the built context
        private BuildContext         _context;

        private MessageWriter        _messageWriter;

        private List<BuildComponent> _components;

        #endregion

        #region Constructors and Destructor

        public BuildAssembler()
            : this(new ConsoleMessageWriter())
        {
        }

        public BuildAssembler(MessageWriter messageWriter)
        {
            if (messageWriter == null)
            {
                throw new ArgumentNullException("messageHandler",
                    "The message writer is required and cannot be null (or Nothing).");
            }

            _messageWriter = messageWriter;
            _context       = new BuildContext();
            _components    = new List<BuildComponent>();
        }

        ~BuildAssembler()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Events

        // component communication mechanism

        public event EventHandler ComponentEvent;

        #endregion

        #region Public Properties

        // data accessors

        public BuildContext Context
        {
            get
            {
                return _context;
            }
        }

        public IList<BuildComponent> BuildComponents
        {
            get
            {
                return _components.ToArray();
            }
        }

        public MessageWriter MessageWriter
        {
            get
            {
                return (_messageWriter);
            }
        }

        public bool IsRaisingEvents
        {
            get
            {
                return (this.ComponentEvent != null);
            }
        }

        #endregion

        #region Public Methods

        public int Apply(string manifestFile)
        {
            return (Apply(new TopicManifest(manifestFile)));
        }

        public int Apply(IEnumerable<string> topics)
        {
            if (_components == null || _components.Count == 0)
            {
                throw new InvalidOperationException(
                    "Build components are not yet loaded.");
            }

            int count = 0;

            foreach (string topic in topics)
            {       
                // create the document
                XmlDocument document = new XmlDocument();
                document.PreserveWhitespace = true;

                // write a log message
                this.WriteMessage(MessageLevel.Info, String.Format(
                    "Building topic {0}", topic));

                // apply the component stack
                foreach (BuildComponent component in _components)
                {  
                    component.Apply(document, topic);

                    if (_messageWriter.ErrorOccurred)
                    {
                        break;
                    }
                }

                if (_messageWriter.ErrorOccurred)
                {
                    break;
                }

                count++;
            }

            return count;
        }

        public IEnumerable<BuildContext> GetFileManifestBuildContextEnumerator(
            string manifestFilename)
        {
            using (XmlReader reader = XmlReader.Create(manifestFilename))
            {
                reader.MoveToContent();
                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Element) && 
                        (reader.LocalName == "topic"))
                    {
                        BuildContext thisContext = new BuildContext();

                        try
                        {
                            string id = reader.GetAttribute("id");

                            while (reader.MoveToNextAttribute())
                            {
                                string name  = reader.Name;
                                string value = reader.Value;
                                thisContext.AddVariable(name, value);
                            }
                        }
                        catch (XmlException e)
                        {
                            throw new XmlException(String.Format(
                                "The manifest file: '{0}' is not well-formed. The error message is: {1}", manifestFilename, e.Message), e);
                        }

                        yield return thisContext;
                    }
                }
            }
        }

        public BuildComponent LoadComponent(XPathNavigator configuration)
        {    
            if (configuration == null) 
                throw new ArgumentNullException("configuration");

            // get the component information
            string assemblyName = configuration.GetAttribute(
                "assembly", String.Empty);
            if (String.IsNullOrEmpty(assemblyName))
            {
                this.WriteMessage(MessageLevel.Error, 
                    "Each component element must have an assembly attribute that specifies a path to the component assembly.");
            }

            string typeName = configuration.GetAttribute("type", String.Empty);
            if (String.IsNullOrEmpty(typeName))
            {
                this.WriteMessage(MessageLevel.Error, "Each component element must have a type attribute that specifys the fully qualified name of a component type.");
            }

            // expand environment variables in path of assembly name
            assemblyName = Environment.ExpandEnvironmentVariables(assemblyName);

            // load and instantiate the component
            BuildComponent component = null;
            try
            { 
                Assembly assembly = Assembly.LoadFrom(assemblyName);
                component = (BuildComponent)assembly.CreateInstance(
                    typeName, false, BindingFlags.Public | BindingFlags.Instance, 
                    null, new Object[2] { this, configuration }, null, null);
            }
            catch (IOException e)
            {
                this.WriteMessage(MessageLevel.Error, String.Format(
                    "A file access error occurred while attempting to load the build component assembly '{0}'. The error message is: {1}", assemblyName, e.Message));
            }
            catch (BadImageFormatException e)
            {
                this.WriteMessage(MessageLevel.Error, String.Format(
                    "The build component assembly '{0}' is not a valid managed assembly. The error message is: {1}", assemblyName, e.Message));
            }
            catch (TypeLoadException)
            {
                this.WriteMessage(MessageLevel.Error, String.Format(
                    "The build component '{0}' was not found in the assembly '{1}'.", typeName, assemblyName));
            }
            catch (MissingMethodException e)
            {
                this.WriteMessage(MessageLevel.Error, String.Format("No appropriate constructor exists for the build component '{0}' in the component assembly '{1}'. The error message is: {1}", typeName, assemblyName, e.Message));
            }
            catch (TargetInvocationException e)
            {
                this.WriteMessage(MessageLevel.Error, String.Format(
                    "An error occurred while initializing the build component '{0}' in the component assembly '{1}'. The error message and stack trace follows: {2}", typeName, assemblyName, e.InnerException.ToString()));
            }
            catch (InvalidCastException)
            {
                WriteMessage(MessageLevel.Error, String.Format(
                    "The type '{0}' in the component assembly '{1}' is not a build component.", typeName, assemblyName));
            }

            if (component == null)
            {
                this.WriteMessage(MessageLevel.Error, String.Format(
                    "The type '{0}' was not found in the component assembly '{1}'.", typeName, assemblyName));
            }

            return (component);
        }

        public IList<BuildComponent> LoadComponents(XPathNavigator configuration)
        {          
            XPathNodeIterator componentNodes = configuration.Select("component");

            List<BuildComponent> components = new List<BuildComponent>();

            foreach (XPathNavigator componentNode in componentNodes)
            {
                components.Add(LoadComponent(componentNode));

                if (_messageWriter.ErrorOccurred)
                {
                    break;
                }
            }

            if (_messageWriter.ErrorOccurred)
            {
                return null;
            }

            return components.ToArray(); 
        }

        // routines to add and remove components from the 

        public void AddComponents(XPathNavigator configuration)
        {
            IList<BuildComponent> componentsToAdd = LoadComponents(configuration);

            if (componentsToAdd == null)
            {
                return;
            }

            foreach (BuildComponent componentToAdd in componentsToAdd)
            {
                _components.Add(componentToAdd);

                if (_messageWriter.ErrorOccurred)
                {
                    break;
                }
            }
        }

        public void ClearComponents()
        {
            _components.Clear();
        }

        public void RaiseComponentEvent(Object o, EventArgs e)
        {
            this.OnComponentEvent(o, e);
        }

        #endregion

        #region Protected Methods

        protected virtual void OnComponentEvent(Object o, EventArgs e)
        {
            if (this.ComponentEvent != null)
            {
                this.ComponentEvent(o, e);
            }
        }

        #endregion

        #region Private Methods

        private void WriteMessage(MessageLevel level, string message)
        {
            if (_thisType == null)
            {
                _thisType = this.GetType();
            }

            _messageWriter.Write(_thisType, level, message);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_components != null)
                {
                    foreach (BuildComponent component in _components)
                    {
                        ((IDisposable)component).Dispose();
                    }

                    _components = null;
                }
            }
        }

        #endregion

        #region TopicManifest Class

        private sealed class TopicManifest : IEnumerable<string>
        {        
            public TopicManifest(string manifest)
            {
                this.manifest = manifest;
            }

            private string manifest;

            public IEnumerator<string> GetEnumerator()
            {
                return (new TopicEnumerator(manifest));
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return (GetEnumerator());
            }
        }

        #endregion

        #region TopicEnumerator Class

        private sealed class TopicEnumerator : IEnumerator<string>
        {
            private XmlReader reader;

            public TopicEnumerator(string manifest)
            {
                reader = XmlReader.Create(manifest);
                reader.MoveToContent();
            }

            public bool MoveNext()
            {
                while (reader.Read())
                {
                    if ((reader.NodeType == XmlNodeType.Element) && (reader.LocalName == "topic")) return (true);
                }
                return (false);
            }

            public string Current
            {
                get
                {
                    if (reader == null)
                    {
                        return null;
                    }

                    return reader.GetAttribute("id");
                }
            }

            Object System.Collections.IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }

            public void Reset()
            {
                throw new InvalidOperationException();
            }

            public void Dispose()
            {
                this.Dispose(true);

                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (disposing)
                {
                    reader.Close();
                }
            }
        }

        #endregion
    }    
}
