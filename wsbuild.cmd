dotnet nuget add source %~dp0/../localnuget --name LOCALFORELMISH

dotnet add .\websharper\WebSharper.Elmish.fsproj package WebSharper --prerelease
dotnet add .\websharper\WebSharper.Elmish.fsproj package WebSharper.FSharp --prerelease

dotnet fsi wsbuild.fsx

copy .\websharper\bin\Release\*.nupkg ..\localnuget


dotnet nuget remove source LOCALFORELMISH
