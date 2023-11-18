namespace AvaloniaExample

open Avalonia.Controls
open Avalonia.Platform.Storage

type FileService(mainWindow: Window) = 
    member this.OpenFilePicker() = 
        mainWindow.StorageProvider.OpenFilePickerAsync(FilePickerOpenOptions())

    member this.TryPickFile() =
        task {
            let! files = this.OpenFilePicker()
            return files |> Seq.tryHead |> Option.map (fun file -> file.Path.AbsolutePath)
        }
