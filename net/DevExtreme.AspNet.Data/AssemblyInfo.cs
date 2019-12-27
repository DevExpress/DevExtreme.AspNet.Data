using System;
using System.Reflection;
using System.Runtime.CompilerServices;

#if DEBUG
[assembly: InternalsVisibleTo("DevExtreme.AspNet.Data.Tests")]
[assembly: InternalsVisibleTo("DevExtreme.AspNet.Data.Tests.Common")]
[assembly: InternalsVisibleTo("DevExtreme.AspNet.Data.Tests.EFCore1")]
[assembly: InternalsVisibleTo("DevExtreme.AspNet.Data.Tests.EFCore2")]
[assembly: InternalsVisibleTo("DevExtreme.AspNet.Data.Tests.EFCore3")]
[assembly: InternalsVisibleTo("DevExtreme.AspNet.Data.Tests.Xpo")]
#endif

[assembly: CLSCompliant(true)]

#if !DEBUG
[assembly: AssemblyKeyFile("release.snk")]
#endif
