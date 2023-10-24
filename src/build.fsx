#r "nuget: Fun.Build, 0.3.8"

open Fun.Build

let root = __SOURCE_DIRECTORY__
let configuration = "Release"
let slnFile = "Elmish.Avalonia.sln"

pipeline "CI" {

    stage "Build" {
        run "dotnet restore"
        run $"dotnet restore {slnFile}"
        run $"dotnet build {slnFile} --configuration {configuration}"
    }

    runIfOnlySpecified false
}

tryPrintPipelineCommandHelp ()