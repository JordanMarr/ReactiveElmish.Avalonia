namespace AvaloniaExample.ViewModels

open Elmish.Avalonia
open Elmish
open Messaging
open AvaloniaExample

module FilePicker = 

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
            { FilePath = path }, Cmd.none
        | Terminate ->
            model, Cmd.none

    let tryPickFile () = 
        let fileProvider = Services.Get<FileService>()
        fileProvider.TryPickFile()

open FilePicker

type FilePickerViewModel() =
    inherit ReactiveElmishViewModel<Model, Msg>(init() |> fst)

    member this.FilePath = this.Bind (_.FilePath >> Option.defaultValue "Not Set")
    member this.Ok() = this.Dispatch Ok
    member this.PickFile() = this.Dispatch PickFile

    override this.StartElmishLoop(view: Avalonia.Controls.Control) = 
        Program.mkAvaloniaProgram init (update tryPickFile)
        |> Program.withErrorHandler (fun (_, ex) -> printfn "Error: %s" ex.Message)
        |> Program.withConsoleTrace
        |> Program.runView this view

    static member DesignVM = new FilePickerViewModel()
