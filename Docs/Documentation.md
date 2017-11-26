The Sandcastle Assist consists of several packages for different developer and user targets. The following are the classifications of the current packages:

### Main Packages
The Sandcastle Assist will consists of a number of packages or assemblies. Currently, we are working on the following:
* **[Sandcastle Common](Sandcastle-Common)**: All the base classes, the utilities classes, configurations classes and more...
* **[Sandcastle Builders](Sandcastle-Builders)**: A complete library for building reference and conceptual documentations. It is a task based build library, and will expand to support both [MSBuild](http://en.wikipedia.org/wiki/MSBuild) and [Team Build](http://msdn.microsoft.com/en-us/library/ms181710(VS.80).aspx).
* **[Sandcastle Build Components](Sandcastle-Build-Components)**: A number of custom build components, which enhance and improve the documentation build system.
* **[Sandcastle Controls](Sandcastle-Controls)**: A number of custom controls to help editing various parts of the Sandcastle configurations and settings. 
* **[Sandcastle Helpers](Sandcastle-Helpers)**: A simple but complete help builder library - **Lite Builders**!

### Application Packages
The Sandcastle Assist will provide ready to use applications for help developments.
* **[Sandcastle Workshop](Sandcastle-Workshop)**:  A complete Integrated Developer Environment (IDE) for creating and compiling help systems using the Sandcastle compiler. This will be the Sandcastle equivalent of the HtmlHelp Workshop.

### Tools Packages
Finally, Sandcastle Assist will provide some utility and tool packages that might be useful to the help developer. These are separated from the main and other packages because of differences in licensing; most of the utilities are based on existing libraries that are under LGPL. These will be place in the **Development** folder in the source control, and will be under the **LGPL license**.
* **[Sandcastle TextEditors](Sandcastle-TextEditors)**: A plain and XML text editors customized to speed up working with help files, especially conceptual help topic files.
* **[Sandcastle Viewers](Sandcastle-Viewers)**: Simple help previewers for compiled helps; the HtmlHelp (**.chm**) and the MSDN Help (**.hxs**).
* **[Sandcastle HelpRegister](Sandcastle-HelpRegister)**: This is a Help 2.0 registration library, written in C#, and it is freely redistributable.
* **[Sandcastle HelpRegister Application](Sandcastle-HelpRegister-Application)**: This is a ready-to-use application for Help 2.0 registration and more.