using System;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("NodaMoney.Serialization.AspNet")]
[assembly: AssemblyDescription("JavaScriptSerializer support for NodaMoney structures.")]
[assembly: NeutralResourcesLanguage("en")]
[assembly: CLSCompliant(true)]
#if !PORTABLE40
[assembly: ComVisible(false)]
#endif
[assembly: InternalsVisibleTo("NodaMoney.Serialization.AspNet.Tests")]
[assembly: Guid("59f9848f-964a-49a6-8a9c-55b737aa5869")]

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
[assembly: AssemblyFileVersion("0.6.1.11")]
[assembly: AssemblyInformationalVersion("0.6.1-dotnet.1+11.Branch.dotnet.Sha.5e1d4e1adae563578333a2fecca7e9d249b26c45")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif
