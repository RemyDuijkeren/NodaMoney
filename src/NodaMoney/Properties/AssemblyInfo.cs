using System;
using System.Resources;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("NodaMoney")]
[assembly: AssemblyDescription("NodaMoney provides a library that treats Money as a first class citizen and handles all the ugly bits like currencies and formatting.")]
[assembly: NeutralResourcesLanguage("en")]
[assembly: CLSCompliant(true)]
#if !PORTABLE40
[assembly: ComVisible(false)]
#endif
[assembly: InternalsVisibleTo("NodaMoney.Tests")]


// Use this SolutionInfo.cs file to store information that is the same in all AssemblyInfo.cs files. Drag this file using ALT to
// your project to create a link to this file.
[assembly: AssemblyProduct("NodaMoney")]
[assembly: AssemblyCompany("DynamicHands B.V.")]
[assembly: AssemblyCopyright("Copyright 2014-2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")] // neutral

//// Please don't change the default version!. The version will be overwritten by the buildserver,
//// using GitVersion (https://github.com/ParticularLabs/GitVersion).
[assembly: AssemblyVersion("0.6.1.0")]
[assembly: AssemblyFileVersion("0.6.1.7")]
[assembly: AssemblyInformationalVersion("0.6.1-dotnet.1+7.Branch.dotnet.Sha.c16b38477d5e4be90406fb4793b97bf07e8afab0")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
