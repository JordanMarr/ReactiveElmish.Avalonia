namespace AvaloniaXPlatExample.Services

open Avalonia.Controls
open Avalonia.Platform.Storage
open System.Threading.Tasks
open System.Collections.Generic

type IFileService =
    abstract member OpenFilePicker: unit -> Task<IReadOnlyList<IStorageFile>>
    abstract member TryPickFile: unit -> Task<string option>

type FileService(mainWindow: Window) = 
    member this.OpenFilePicker() = 
        mainWindow.StorageProvider.OpenFilePickerAsync(FilePickerOpenOptions())

    member this.TryPickFile() =
        task {
            let! files = this.OpenFilePicker()
            return files |> Seq.tryHead |> Option.map (fun file -> file.Path.AbsolutePath)
        }

    interface IFileService with
        member this.OpenFilePicker() = this.OpenFilePicker()
        member this.TryPickFile() = this.TryPickFile()

type WebFileService() = 
    member this.OpenFilePicker() = Task.FromResult Unchecked.defaultof<_>
    member this.TryPickFile() = Task.FromResult None
    interface IFileService with
        member this.OpenFilePicker() = this.OpenFilePicker()
        member this.TryPickFile() = this.TryPickFile()
