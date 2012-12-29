using System;
using System.Collections.Generic;

namespace XsdDocumentation.Model
{
    public sealed class Configuration
    {
        public Configuration()
        {
            this.DocFileNames = new List<string>();
            this.SchemaFileNames = new List<string>();
            this.SchemaDependencyFileNames = new List<string>();

            this.NamespaceRenaming = new Dictionary<string, string>(
                StringComparer.Ordinal);

            this.IncludeMoveToTop = true;
        }

        public string OutputFolderPath { get; set; }
        public bool DocumentRootSchemas { get; set; }
        public bool DocumentRootElements { get; set; }
        public bool DocumentConstraints { get; set; }
        public bool DocumentSchemas { get; set; }
        public bool DocumentSyntax { get; set; }
        public bool UseTypeDocumentationForUndocumentedAttributes { get; set; }
        public bool UseTypeDocumentationForUndocumentedElements { get; set; }
        public bool SchemaSetContainer { get; set; }
        public string SchemaSetTitle { get; set; }
        public bool NamespaceContainer { get; set; }
        public bool IncludeLinkUriInKeywordK { get; set; }
        public bool IncludeAutoOutline { get; set; }
        public bool IncludeMoveToTop { get; set; }
        public string AnnotationTransformFileName { get; set; }

        public IList<string> SchemaFileNames { get; set; }
        public IList<string> SchemaDependencyFileNames { get; set; }
        public IList<string> DocFileNames { get; set; }

        public IDictionary<string, string> NamespaceRenaming { get; set; }
    }
}