using System;
using System.Reflection;
using System.Runtime.CompilerServices;

#if DEBUG
[assembly: InternalsVisibleTo("DevExtreme.AspNet.Data.Tests")]
[assembly: InternalsVisibleTo("DevExtreme.AspNet.Data.Tests.EF6")]
[assembly: InternalsVisibleTo("DevExtreme.AspNet.Data.Tests.EFCore1")]
#endif

[assembly: CLSCompliant(true)]

#if !DEBUG
[assembly: AssemblyKeyFile("release.snk")]
#endif
