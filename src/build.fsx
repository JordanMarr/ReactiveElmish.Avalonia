#r "nuget: Fun.Build, 0.3.8"

open Fun.Build

let src = __SOURCE_DIRECTORY__

pipeline "CI" {

    stage "Build Elmish.Avalonia" {
        run $"dotnet restore {src}/Elmish.Avalonia.sln"
        run $"dotnet build {src}/Elmish.Avalonia.sln --configuration Release"
    }
    
    stage "Build AvaloniaXPlatExample" {
        //run $"dotnet workload restore {src}/Samples/AvaloniaXPlatExample/AvaloniaXPlatExample.sln"
        run $"dotnet workload restore {src}/Samples/AvaloniaXPlatExample/AvaloniaXPlatExample.Android/AvaloniaXPlatExample.Android.fsproj"
        run $"dotnet workload restore {src}/Samples/AvaloniaXPlatExample/AvaloniaXPlatExample.Desktop/AvaloniaXPlatExample.Desktop.fsproj"
        run $"dotnet workload restore {src}/Samples/AvaloniaXPlatExample/AvaloniaXPlatExample.iOS/AvaloniaXPlatExample.iOS.csproj"
        run $"dotnet workload restore {src}/Samples/AvaloniaXPlatExample/AvaloniaXPlatExample.Web/AvaloniaXPlatExample.Web.fsproj"
        run $"dotnet restore {src}/Samples/AvaloniaXPlatExample/AvaloniaXPlatExample.sln"
        run $"dotnet build {src}/Samples/AvaloniaXPlatExample/AvaloniaXPlatExample.sln --configuration Release"
    }

    runIfOnlySpecified false
}

tryPrintPipelineCommandHelp ()