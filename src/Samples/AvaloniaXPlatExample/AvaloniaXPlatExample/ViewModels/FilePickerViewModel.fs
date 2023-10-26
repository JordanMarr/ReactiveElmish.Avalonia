module AvaloniaXPlatExample.ViewModels.FilePickerViewModel

open Elmish.Avalonia
open Elmish
open Messaging
open AvaloniaXPlatExample

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

let bindings ()  : Binding<Model, Msg> list = [
    "Ok" |> Binding.cmd Ok
    "FilePath" |> Binding.oneWay (fun m -> m.FilePath |> Option.defaultValue "Not Set")
    "PickFile" |> Binding.cmd PickFile
]

let designVM =
    ViewModel.designInstance (fst (init ())) (bindings())

let vm () =
    let tryPickFile () =
        let fileProvider = Services.Get<FileService>()
        fileProvider.TryPickFile()

    AvaloniaProgram.mkProgram init (update tryPickFile) bindings
    |> ElmishViewModel.create
