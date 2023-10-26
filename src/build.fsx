#r "nuget: Fun.Build, 1.0.2"

open Fun.Build

let src = __SOURCE_DIRECTORY__

pipeline "CI" {

    stage "Build Elmish.Avalonia" {
        run $"dotnet restore {src}/Elmish.Avalonia.sln"
        run $"dotnet build {src}/Elmish.Avalonia.sln --configuration Release"
    }
    
    stage "Build AvaloniaXPlatExample" {
        run "dotnet workload install android"
        run "dotnet workload install wasm-tools"
        run $"dotnet workload restore {src}/Samples/AvaloniaXPlatExample/AvaloniaXPlatExample.sln"
        run $"dotnet restore {src}/Samples/AvaloniaXPlatExample/AvaloniaXPlatExample.sln"
        run $"dotnet build {src}/Samples/AvaloniaXPlatExample/AvaloniaXPlatExample.sln --configuration Debug"
    }

    runIfOnlySpecified false
}

tryPrintPipelineCommandHelp ()