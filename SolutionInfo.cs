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

//// For versioning, see http://stackoverflow.com/questions/64602/what-are-differences-between-assemblyversion-assemblyfileversion-and-assemblyinf/65062#65062
//// and Semantic Versioning (http://semver.org/spec/v2.0.0.html)

//// Where other assemblies that reference your assembly will look at. If this number changes, your references have to be updated. (major.minor)
[assembly: AssemblyVersion("0.1")]
//// Used for deployment. You can increase this number for every deployment. It is used by setup programs. (major.minor.revision.build)
[assembly: AssemblyFileVersion("0.1.1000.0")]
//// Used when talking to customers or for display on your website. (major.minor (revision as string))
[assembly: AssemblyInformationalVersion("0.1 Alpha1")]