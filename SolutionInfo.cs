using System.Reflection;

[assembly: AssemblyProduct("NodaMoney")]
[assembly: AssemblyCompany("DynamicHands B.V.")]
[assembly: AssemblyCopyright("Copyright 2014-2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")] // neutral

//// Please don't change the default version!. The version will be overwritten by the buildserver, 
//// using GitVersion (https://github.com/ParticularLabs/GitVersion).
[assembly: AssemblyVersion("0.0.0.0")]
[assembly: AssemblyFileVersion("0.0.0.0")]
[assembly: AssemblyInformationalVersion("0.0.0.0")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif