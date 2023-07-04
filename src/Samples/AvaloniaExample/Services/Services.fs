namespace AvaloniaExample

open System
open Avalonia.Controls
open Microsoft.Extensions.DependencyInjection
open Avalonia.Platform.Storage

type FileService(mainWindow: Window) = 
    member this.OpenFilePicker() = 
        mainWindow.StorageProvider.OpenFilePickerAsync(FilePickerOpenOptions())

    member this.TryPickFile() =
        task {
            let! files = this.OpenFilePicker()
            return files |> Seq.tryHead |> Option.map (fun file -> file.Path.AbsolutePath)
        }

type Services() = 
    static let mutable container : IServiceProvider = null
        
    static member Container 
        with get() = container

    static member Init mainWindow = 
        let services = ServiceCollection()
        services.AddSingleton<FileService>(FileService(mainWindow)) |> ignore
        container <- services.BuildServiceProvider()

    static member Get<'Svc>() = 
        container.GetRequiredService<'Svc>()