dotnet add .\websharper\WebSharper.Elmish.fsproj package WebSharper --prerelease
dotnet add .\websharper\WebSharper.Elmish.fsproj package WebSharper.FSharp --prerelease

dotnet fsi wsbuild.fsx

cp .\websharper\bin\Release\*.nupkg ..\localnuget
