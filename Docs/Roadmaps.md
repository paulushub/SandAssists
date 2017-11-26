## Development Roadmap
We are currently working on a number of projects for [Sandcastle](http://sandcastle.codeplex.com/) help system developments. The development and enhancements of the [Sandcastle Workshop](Sandcastle-Workshop) (our IDE for developing Sandcastle-based help system) is making a lot of progress. We have finally decided to present the roadmap.

Basically, we are targeting the release of the Microsoft VS.NET 2010, making a help building system available as you take off.

**NOTE**: With the extension of the VS.NET 2010 release date, we are now reviewing this roadmap. A new roadmap will be published when more information is released on the VS.NET 2010 release schedule.

### 1. Sandcastle Workshop
An Integrated Developer Environment for developing Windows help systems using the [Sandcastle](http://sandcastle.codeplex.com/) compiler system.
#### Alpha: December 15th, 2009
This is the first general release of the [Sandcastle Workshop](Sandcastle-Workshop).
* No compilation of help is planned in this release.
* Complete templates for Sandcastle conceptual help and related topic files
* Startup page for the IDE
* Early stages of the **Sandcastle.Workshop** (Alpha Add-In). This is the core of our Sandcastle specific IDE addin to the Sandcastle Workshop.
* Start of the **Sandcastle.Workshop.Builders** (Pre-Alpha Add-In).
* Sources checked into Codeplex source control

#### Beta: January 30th, 2010
This is first public beta release, with some Sandcastle specific tools integrated into the IDE.
* Integrate a modified version of [ScintillaNET](http://scintillanet.codeplex.com/), minimum integration as Output/Build Window
* Complete **Sandcastle.Workshop**, will provide the editing of contents (Beta)
* More features implementation for the **Sandcastle.Workshop.Builders** (Beta)
* Start of documentations, the following will be covered
	* Sandcastle documentation, 
	* Sandcastle Workshop documentation,
	* Sandcastle Builder documentation
	* Other related user and reference manuals
* Start of **Sandcastle.Workshop.Tools** (Alpha/Beta Add-In)
NOTE: There may be more than one beta releases.

#### Release Candidate: February 27th, 2010
A more feature complete release of the building system, and the integrated developer environment.
* Improved documentation
* Start of **Sandcastle.Workshop.Resources** for localization.
NOTE: There may be more than one release candidates.

#### Final Release (RTW): March 31st, 2010
* The first major release of the help building system, the integrated developer environment for authoring and the user manual and references.
* Completed Sandcastle Workshop Documentation

----

### 2. Sandcastle Builders and Helpers
These are our help building libraries. The [Sandcastle Helpers](Sandcastle-Helpers) library is a smaller version for most projects, the [Sandcastle Builders](Sandcastle-Builders) library extends this and provides an add-in framework for community based extensions.
#### Beta: January 15th, 2009
* A working help building library, supporting Help 1.x, Help 2.x, Help 3.x (if released by Sandcastle Team by then), and web help.
* Introduction of plug-in framework (based on [Mono.AddIn](http://www.mono-project.com/Mono.Addins), which is winning over Microsoft [MEF](http://mef.codeplex.com/), since it works with .NET 2.x).
* More work on the [Sandcastle Build Components](Sandcastle-Build-Components).

#### Release Candidate: February 15th, 2010
* Multiple language support (localizations)
* Help and Documentations
* Completion of the [Sandcastle Build Components](Sandcastle-Build-Components).

#### Final Release (RTW): March 31st, 2010
* The first release of our Sandcastle Helpers and Builders libraries and related tools.
* Complete documentation of the libraries.