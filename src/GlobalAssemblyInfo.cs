using System.Reflection;

#if DEBUG
[assembly: AssemblyConfiguration ("DEBUG")]
#endif
#if RELEASE
[assembly: AssemblyConfiguration ("RELEASE")]
#endif

[assembly: AssemblyCompany ("moq")]
[assembly: AssemblyProduct ("moq.proxy")]
[assembly: AssemblyCopyright ("Copyright © 2016 Daniel Cazzulino")]

#pragma warning disable 0436
[assembly: AssemblyVersion (ThisAssembly.Git.SemVer.Major + "." + ThisAssembly.Git.SemVer.Minor + "." + ThisAssembly.Git.SemVer.Patch)]
[assembly: AssemblyFileVersion (ThisAssembly.Git.SemVer.Major + "." + ThisAssembly.Git.SemVer.Minor + "." + ThisAssembly.Git.SemVer.Patch)]
[assembly: AssemblyInformationalVersion (ThisAssembly.Git.SemVer.Major + "." + ThisAssembly.Git.SemVer.Minor + "." + ThisAssembly.Git.SemVer.Patch + "-" + ThisAssembly.Git.Branch + "+" + ThisAssembly.Git.Commit)]
#pragma warning restore 0436