using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Helpers.Sample
{
    public class UserOptions
    {
        private BuildSettings _settings;

        public UserOptions(BuildSettings settings)
        {
            BuildExceptions.NotNull(settings, "settings");

            _settings = settings;
        }

        public bool Run()
        {
            //_settings.HelpName = "HelpRegister";
            //_settings.HelpTitle = "Sandcastle HelpRegister";
            _settings.HelpName = "SandcastleHelpers";
            _settings.HelpTitle = "Sandcastle Helpers Test Sample";
 
            bool selectBuild = SelectBuildType();

            return selectBuild;
        }

        #region SelectBuildType Method

        private bool SelectBuildType()
        {
            Console.Title = "Sandcastle Test Builder - " + _settings.Style.StyleType;
            Console.SetWindowSize(Console.LargestWindowWidth - 20,
                Console.LargestWindowHeight - 20);
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();

            Console.WriteLine(
                "******** This is Sandcastle Test Sample *************");
            Console.WriteLine();
            Console.WriteLine("The Output Style: " + _settings.Style.StyleType);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(
                "Please selection the build option: typing 0, 1, 2 or 3");
            Console.WriteLine("\t0 -- Quit or Exit");
            Console.WriteLine("\t1 -- Build Reference Help");
            Console.WriteLine("\t2 -- Build Conceptual Help");
            Console.WriteLine("\t3 -- Build both Conceptual and Reference Help");
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write("\t\t?>>>");
            bool isInvalid = true;
            int buildType = -1;
            while (isInvalid)
            {
                string textRead = Console.ReadLine();
                int option = -1;
                if (String.IsNullOrEmpty(textRead) == false)
                {
                    try
                    {
                        option = Convert.ToInt32(textRead);
                        isInvalid = (option < 0 || option > 3);
                    }
                    catch
                    {
                        isInvalid = true;
                    }
                }

                if (isInvalid)
                {
                    Console.Write("\t\t?>>>");
                }
                else
                {
                    buildType = option;
                    Console.Clear();
                }
            }
            if (buildType <= 0)  // quit or exit
            {
                return false;
            }
            else if (buildType == 1) // reference build
            {
                _settings.BuildReferences = true;
                _settings.BuildConceptual = false;
            }
            else if (buildType == 2) // conceptual build
            {
                _settings.BuildReferences = false;
                _settings.BuildConceptual = true;
            }
            else if (buildType == 3) // reference and conceptual builds
            {
                _settings.BuildReferences = true;
                _settings.BuildConceptual = true;
            }
            else
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
