#r "nuget: Fun.Build, 1.0.2"

open Fun.Build

let src = __SOURCE_DIRECTORY__

pipeline "CI Build" {

    stage "Build ReactiveElmish.sln" {
        run $"dotnet restore {src}/ReactiveElmish.sln"
        run $"dotnet build {src}/ReactiveElmish.sln --configuration Release"
    }
    
    stage "Build AvaloniaXPlatExample.sln" {
        run "dotnet workload install android"
        run "dotnet workload install wasm-tools"
        run $"dotnet workload restore {src}/Samples/AvaloniaXPlatExample/AvaloniaXPlatExample.sln"
        run $"dotnet restore {src}/Samples/AvaloniaXPlatExample/AvaloniaXPlatExample.sln"
        run $"dotnet build {src}/Samples/AvaloniaXPlatExample/AvaloniaXPlatExample.sln --configuration Debug"
    }

    runIfOnlySpecified false
}

tryPrintPipelineCommandHelp ()