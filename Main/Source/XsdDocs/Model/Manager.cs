using System;

namespace XsdDocumentation.Model
{
    public abstract class Manager
    {
        protected Manager(Context context)
        {
            this.Context = context;
        }

        public abstract void Initialize();

        public Context Context { get; private set; }
    }
}