namespace AvaloniaExample

open System
open Avalonia.Controls
open Microsoft.Extensions.DependencyInjection
open Avalonia.Platform.Storage

type FileService(window: Window) = 
    member this.OpenFilePicker() = 
        window.StorageProvider.OpenFilePickerAsync(FilePickerOpenOptions())

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