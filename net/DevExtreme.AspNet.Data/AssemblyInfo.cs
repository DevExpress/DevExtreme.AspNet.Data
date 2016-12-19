﻿using System;
using System.Reflection;
using System.Runtime.CompilerServices;

#if DEBUG
[assembly: InternalsVisibleTo("DevExtreme.AspNet.Data.Tests")]
#endif

[assembly: AssemblyVersion("99.0")]
[assembly: AssemblyCompany("%meta_company%")]
[assembly: AssemblyCopyright("%meta_copyright%")]
[assembly: AssemblyDescription("%meta_description%")]
[assembly: AssemblyProduct("DevExtreme.AspNet.Data")]
[assembly: CLSCompliant(true)]

#if !DEBUG
[assembly: AssemblyKeyFile("release.snk")]
#endif