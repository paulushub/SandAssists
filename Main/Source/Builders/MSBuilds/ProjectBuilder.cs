using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Sandcastle.Builders.MSBuilds
{
    public sealed class ProjectBuilder : Task
    {
        public ProjectBuilder()
        {   
        }

        public override bool Execute()
        {
            Log.LogMessageFromText("ProjectBuilder: Testing Builder!", 
                MessageImportance.High);

            return true;
        }
    }
}
