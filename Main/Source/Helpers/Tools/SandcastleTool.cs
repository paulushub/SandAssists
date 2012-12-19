using System;

namespace Sandcastle.Tools
{
    public abstract class SandcastleTool : BuildTool
    {
        protected SandcastleTool()
        {   
        } 
        
        public override bool Run(BuildContext context)
        {
            return this.OnRun(context.Logger);
        }

        protected abstract bool OnRun(BuildLogger logger);
    }
}
