#!/usr/bin/env -S dotnet fsi
#r "nuget: Fake.Core.Target"
#r "nuget: Fake.IO.FileSystem"
#r "nuget: Fake.DotNet.Cli"
#r "nuget: Fake.Core.Target"
#r "nuget: Fake.Core.ReleaseNotes"
#r "nuget: Fake.Tools.Git"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.Tools
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open System
open System.IO


// Filesets
let projects  = !! "websharper/**.fsproj"


System.Environment.GetCommandLineArgs() 
|> Array.skip 2 // fsi.exe; build.fsx
|> Array.toList
|> Context.FakeExecutionContext.Create false __SOURCE_FILE__
|> Context.RuntimeContext.Fake
|> Context.setExecutionContext

Target.create "Clean" (fun _ ->
    Shell.cleanDir "src/obj"
    Shell.cleanDir "src/bin"
    Shell.cleanDir "netstandard/obj"
    Shell.cleanDir "netstandard/bin"
    Shell.cleanDir "websharper/obj"
    Shell.cleanDir "websharper/bin"
)

Target.create "Restore" (fun _ ->
    projects
    |> Seq.iter (Path.GetDirectoryName >> DotNet.restore id)
)

Target.create "Build" (fun _ ->
    projects
    |> Seq.iter (Path.GetDirectoryName >> DotNet.build id)
)

let release = ReleaseNotes.load "RELEASE_NOTES.md"

Target.create "Meta" (fun _ ->
    [ "<Project xmlns=\"http://schemas.microsoft.com/developer/msbuild/2003\">"
      "<ItemGroup>"
      "<None Include=\"../docs/static/img/logo.png\" Pack=\"true\" PackagePath=\"\\\"/>"
      "<PackageReference Include=\"Microsoft.SourceLink.GitHub\" Version=\"1.0.0\" PrivateAssets=\"All\"/>"
      "</ItemGroup>"
      "<PropertyGroup>"
      "<EmbedUntrackedSources>true</EmbedUntrackedSources>"
      "<AllowedOutputExtensionsInPackageBuildOutputFolder>$(AllowedOutputExtensionsInPackageBuildOutputFolder);.pdb</AllowedOutputExtensionsInPackageBuildOutputFolder>"
      "<PackageProjectUrl>https://github.com/elmish/elmish</PackageProjectUrl>"
      "<PackageLicenseExpression>Apache-2.0</PackageLicenseExpression>"
      "<PackageIconUrl>https://raw.githubusercontent.com/elmish/elmish/master/docs/files/img/logo.png</PackageIconUrl>"
      "<PackageIcon>logo.png</PackageIcon>"
      "<RepositoryUrl>https://github.com/elmish/elmish.git</RepositoryUrl>"
      sprintf "<PackageReleaseNotes>%s</PackageReleaseNotes>" (List.head release.Notes)
      "<PackageTags>MVU;fsharp</PackageTags>"
      "<Authors>Eugene Tolmachev</Authors>"
      sprintf "<Version>%s</Version>" (string release.SemVer)
      "</PropertyGroup>"
      "</Project>"]
    |> File.write false "Directory.Build.props"
)

// --------------------------------------------------------------------------------------
// Build a NuGet package

Target.create "Package" (fun _ ->
    projects
    |> Seq.iter (Path.GetDirectoryName >> DotNet.pack id)
)

Target.create "WS-Package" ignore

Target.create "CI-Tag" ignore

Target.create "CI-Commit" ignore

// Build order
"Clean"
    ==> "Meta"
    ==> "Restore"
    ==> "Build"
    ==> "Package"
    ==> "WS-Package"

// start build
Target.runOrDefault "WS-Package"
