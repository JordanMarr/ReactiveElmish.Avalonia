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

        //run $"dotnet workload restore {src}/Samples/AvaloniaXPlatExample/AvaloniaXPlatExample.sln"
        // Restore projects individually because the CI build recently started deriving incorrect project paths when restoring at the sln level.
        run $"dotnet workload restore {src}/Samples/AvaloniaXPlatExample/AvaloniaXPlatExample.Android/AvaloniaXPlatExample.Android.fsproj"
        run $"dotnet workload restore {src}/Samples/AvaloniaXPlatExample/AvaloniaXPlatExample.Desktop/AvaloniaXPlatExample.Desktop.fsproj"
        run $"dotnet workload restore {src}/Samples/AvaloniaXPlatExample/AvaloniaXPlatExample.iOS/AvaloniaXPlatExample.iOS.csproj"
        run $"dotnet workload restore {src}/Samples/AvaloniaXPlatExample/AvaloniaXPlatExample.Web/AvaloniaXPlatExample.Web.fsproj"

        run $"dotnet restore {src}/Samples/AvaloniaXPlatExample/AvaloniaXPlatExample.sln"
        run $"dotnet build {src}/Samples/AvaloniaXPlatExample/AvaloniaXPlatExample.sln --configuration Debug"
    }

    runIfOnlySpecified false
}

tryPrintPipelineCommandHelp ()