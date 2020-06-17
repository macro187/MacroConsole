using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;


namespace
MacroConsole
{


public static class
ShadowCopier
{


const string
ShadowDirName = "_shadow";


/// <summary>
/// "Shadow copy" the calling program's binaries to a shadow subdirectory
/// </summary>
///
/// <remarks>
/// <para>
/// Actually, move the calling program's running binaries to a <c>_shadow</c> subdirectory, maintaining unlocked copies
/// in the original locations which can be replaced or removed while the program is still running.
/// </para>
/// <para>
/// Prior to moving, all assemblies statically referenced by the calling program are preloaded so the running process
/// binds to them, otherwise it may lazy load and bind after the copy is perfomed, defeating the purpose.
/// </para>
/// <para>
/// Shortcomings:
/// - Program requires write access to its binaries' location on disk
/// - The copy is shallow, it does not recurse into subdirectories
/// - Probably fails if more than one instance of the program is run at the same time, because both processes would try
///   to use the same shadow subdirectory
/// </para>
/// </remarks>
///
public static void
ShadowCopy()
{
    var callingAssembly = Assembly.GetCallingAssembly();

    PreloadReferencedAssemblies(callingAssembly);

    var originalDir = Path.GetDirectoryName(callingAssembly.Location);
    var shadowDir = Path.Combine(originalDir, ShadowDirName);
    if (Directory.Exists(shadowDir)) Directory.Delete(shadowDir, true);
    Directory.CreateDirectory(shadowDir);

    foreach (var path in Directory.EnumerateFiles(originalDir))
    {
        var name = Path.GetFileName(path);
        var shadowPath = Path.Combine(shadowDir, name);
        File.Move(path, shadowPath);
    }

    foreach (var path in Directory.EnumerateFiles(shadowDir))
    {
        var name = Path.GetFileName(path);
        var originalPath = Path.Combine(originalDir, name);
        File.Copy(path, originalPath);
    }
}


static void
PreloadReferencedAssemblies(Assembly assembly)
{
    PreloadReferencedAssemblies(assembly, new HashSet<string>());
}


static void
PreloadReferencedAssemblies(Assembly assembly, ISet<string> loaded)
{
    foreach (var assemblyName in assembly.GetReferencedAssemblies())
    {
        if (loaded.Contains(assemblyName.Name)) continue;
        loaded.Add(assemblyName.Name);
        PreloadReferencedAssemblies(Assembly.Load(assemblyName), loaded);
    }
}


}
}
