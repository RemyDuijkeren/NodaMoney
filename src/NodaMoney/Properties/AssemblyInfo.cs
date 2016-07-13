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