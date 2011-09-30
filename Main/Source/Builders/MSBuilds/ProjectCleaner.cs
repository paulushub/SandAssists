using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Sandcastle.Builders.MSBuilds
{
    public sealed class ProjectCleaner : Task
    {
        public ProjectCleaner()
        {
        }

        public override bool Execute()
        {
            Log.LogMessageFromText("ProjectCleaner: Testing Cleaner!",
                MessageImportance.High);

            return true;
        }
    }
}
