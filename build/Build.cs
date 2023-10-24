using System.IO;
using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Tools.DotNet.DotNetTasks;


class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Publish);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;
    
    [Solution] readonly Solution Solution;
    
    AbsolutePath SourceDirectory => RootDirectory / "source";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath OutputDirectory => RootDirectory / "publish";

    Target Clean => t => t
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(x => x.DeleteDirectory());
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(x => x.DeleteDirectory());
            OutputDirectory.CreateOrCleanDirectory();
        });

    Target Restore => t => t
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => t => t
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target Publish => t => t
        .DependsOn(Compile)
        .Executes(() =>
        {
            const string version = "0.1.0";
            const string authors = "Tobias Brinke";
            const string tags = "EntityFramework";
            
            DotNetPack(s => s
                .SetProject(Solution.GetProject("EfExtensions.Core"))
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore()
                .SetVersion(version)
                .SetAuthors(authors)
                .SetTitle("EfExtensions.Core")
                .SetDescription("Core package for the EfExtensions Nuget Packages. Provides interfaces and necessary core items.")
                .SetPackageTags(tags)
                .SetNoDependencies(true)
                .SetOutputDirectory(OutputDirectory));
            
            DotNetPack(s => s
                .SetProject(Solution.GetProject("EfExtensions.Items"))
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore()
                .SetVersion(version)
                .SetAuthors(authors)
                .SetTitle("EfExtensions.Items")
                .SetDescription("Provides implementations for the item interfaces from the EfExtensions.Core package.")
                .SetPackageTags(tags)
                .SetNoDependencies(true)
                .SetOutputDirectory(OutputDirectory));
            
            DotNetPack(s => s
                .SetProject(Solution.GetProject("EfExtensions.Repositories"))
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableNoRestore()
                .SetVersion(version)
                .SetAuthors(authors)
                .SetTitle("EfExtensions.Repositories")
                .SetDescription("Provides implementations for the repository interfaces from the EfExtensions.Core packages.")
                .SetPackageTags(tags)
                .SetNoDependencies(true)
                .SetOutputDirectory(OutputDirectory));
        });
}
