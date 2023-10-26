#r "nuget: Fun.Build, 1.0.2"
#r "nuget: Fake.Core.Environment, 5.22.0"

open System.IO
open System.Xml.Linq
open Fun.Build
open Fake.Core

let src = __SOURCE_DIRECTORY__

pipeline "Publish" {

    stage "Build Elmish.Avalonia" {
        run $"dotnet restore {src}/Elmish.Avalonia.sln"
        run $"dotnet build {src}/Elmish.Avalonia.sln --configuration Release"
    }
    
    stage "Publish Elmish.Avalonia" {
        run (fun _ -> 
            let version = 
                let project = (FileInfo $"{src}/Elmish.Avalonia/Elmish.Avalonia.fsproj")
                let doc = XDocument.Load(project.FullName)
                match doc.Descendants("Version") |> Seq.tryHead with
                | Some versionElement -> versionElement.Value
                | None -> failwith $"Could not find a <Version> element in '{project.Name}'."

            let nugetKey =
                match Environment.environVarOrNone "ELMISH_AVALONIA_NUGET_KEY" with
                | Some nugetKey -> nugetKey
                | None -> failwith "The NuGet API key must be set in an 'ELMISH_AVALONIA_NUGET_KEY' environmental variable"
            
            $"dotnet nuget push \"{src}/Elmish.Avalonia/bin/Release/Elmish.Avalonia.{version}.nupkg\" -s nuget.org -k {nugetKey} --skip-duplicate"
        )
    }

    runIfOnlySpecified false
}

tryPrintPipelineCommandHelp ()