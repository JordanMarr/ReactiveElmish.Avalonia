module AvaloniaExample.ViewModels.FilePickerViewModel

open Elmish.Avalonia
open Elmish
open Messaging
open AvaloniaExample

type Model = 
    {
        FilePath: string option
    }

type Msg = 
    | Ok
    | PickFile
    | SetFilePath of string option
    | Terminate

let init () = 
    { 
        FilePath = None
    }, Cmd.none

let update tryPickFile (msg: Msg) (model: Model) = 
    match msg with
    | Ok -> 
        model, Cmd.ofEffect (fun _ -> bus.OnNext(GlobalMsg.GoHome))
    | PickFile  -> 
        model, Cmd.OfTask.perform tryPickFile () SetFilePath
    | SetFilePath path ->
        { model with FilePath = path }, Cmd.none
    | Terminate ->
        model, Cmd.none

let tryPickFile () = 
    let fileProvider = Services.Get<FileService>()
    fileProvider.TryPickFile()

type FilePickerViewModel() =
    inherit ReactiveElmishViewModel<Model, Msg>(init() |> fst)

    member this.FilePath = this.BindModel(nameof this.FilePath, fun m -> m.FilePath |> Option.defaultValue "Not Set")
    member this.Ok() = this.Dispatch Msg.Ok
    member this.PickFile() = this.Dispatch Msg.PickFile

    override this.StartElmishLoop(view: Avalonia.Controls.Control) = 
        Program.mkAvaloniaProgram init (update tryPickFile)
        |> Program.withErrorHandler (fun (_, ex) -> printfn "Error: %s" ex.Message)
        |> Program.withConsoleTrace
        |> Program.runView this view

let designVM = new FilePickerViewModel()
