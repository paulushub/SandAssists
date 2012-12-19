using System;

namespace Sandcastle
{
    public abstract class BuildTool : MarshalByRefObject
    {
        protected BuildTool()
        {
        }

        public abstract bool Run(BuildContext context);

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
