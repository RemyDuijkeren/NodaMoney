using System;
using System.Reflection;

[assembly: AssemblyProduct("NodaMoney")]
[assembly: AssemblyCompany("DynamicHands B.V.")]
[assembly: AssemblyCopyright("Copyright © 2014-2015")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")] // neutral

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif