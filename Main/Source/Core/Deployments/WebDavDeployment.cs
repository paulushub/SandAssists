using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sandcastle.Deployments
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class WebDavDeployment : BuildDeployment
    {
        public WebDavDeployment()
        {
        }

        public override bool IsValid
        {
            get { throw new NotImplementedException(); }
        }

        public override void BeginDeployment(BuildLogger logger)
        {
            throw new NotImplementedException();
        }

        public override void Deployment(BuildLogger logger, BuildDirectoryPath sourcePath)
        {
            throw new NotImplementedException();
        }

        public override void EndDeployment(BuildLogger logger)
        {
            throw new NotImplementedException();
        }

        public override BuildDeployment Clone()
        {
            throw new NotImplementedException();
        }
    }
}
