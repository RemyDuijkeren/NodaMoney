using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if !PORTABLE
using System.Resources;
[assembly: NeutralResourcesLanguage("en")]
#endif

[assembly: AssemblyTitle("NodaMoney")]
[assembly: AssemblyDescription("NodaMoney provides a library that treats Money as a first class citizen and handles all the ugly bits like currencies and formatting.")]
[assembly: CLSCompliant(true)]
[assembly: InternalsVisibleTo("NodaMoney.Tests")]
#if !PORTABLE40
[assembly: ComVisible(false)]
#endif


// Use this SolutionInfo.cs file to store information that is the same in all AssemblyInfo.cs files. Drag this file using ALT to
// your project to create a link to this file.
[assembly: AssemblyProduct("NodaMoney")]
[assembly: AssemblyCompany("DynamicHands B.V.")]
[assembly: AssemblyCopyright("Copyright 2014-2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")] // neutral

//// Please don't change the default version!. The version will be overwritten by the buildserver,
//// using GitVersion (https://github.com/ParticularLabs/GitVersion).
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
[assembly: AssemblyInformationalVersion("1.0.0-beta.0")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
