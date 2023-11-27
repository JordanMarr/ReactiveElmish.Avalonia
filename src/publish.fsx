#r "nuget: Fun.Build, 1.0.2"

open System.IO
open System.Xml.Linq
open Fun.Build

let src = __SOURCE_DIRECTORY__

pipeline "Publish" {

    stage "Build Elmish.Avalonia" {
        run $"dotnet restore {src}/Elmish.Avalonia.sln"
        run $"dotnet build {src}/Elmish.Avalonia.sln --configuration Release"
    }
    
    stage "Publish Elmish.Avalonia" {
        run (fun ctx ->             
            let version = 
                let project = (FileInfo $"{src}/Elmish.Avalonia/Elmish.Avalonia.fsproj")
                match XDocument.Load(project.FullName).Descendants("Version") |> Seq.tryHead with
                | Some versionElement -> versionElement.Value
                | None -> failwith $"Could not find a <Version> element in '{project.Name}'."
            
            let nugetKey = 
                match ctx.TryGetEnvVar "REACTIVE_ELMISH_NUGET_KEY" with
                | ValueSome nugetKey -> nugetKey
                | ValueNone -> failwith "The NuGet API key must be set in an 'REACTIVE_ELMISH_NUGET_KEY' environmental variable"
            
            $"dotnet nuget push \"{src}/Elmish.Avalonia/bin/Release/Elmish.Avalonia.{version}.nupkg\" -s nuget.org -k {nugetKey} --skip-duplicate"
        )
    }

    runIfOnlySpecified false
}

tryPrintPipelineCommandHelp ()