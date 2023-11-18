namespace AvaloniaExample.ViewModels

open Elmish.Avalonia
open Elmish
open AvaloniaExample

module FilePicker = 

    type Model = 
        {
            FilePath: string option
        }

    type Msg = 
        | PickFile
        | SetFilePath of string option

    let init () = 
        { 
            FilePath = None
        }, Cmd.none

    let update tryPickFile (msg: Msg) (model: Model) = 
        match msg with
        | PickFile  -> 
            model, Cmd.OfTask.perform tryPickFile () SetFilePath
        | SetFilePath path ->
            { FilePath = path }, Cmd.none

    let tryPickFile () = 
        let fileProvider = Services.Get<FileService>()
        fileProvider.TryPickFile()

open FilePicker

type FilePickerViewModel(app: IElmishStore<App.Model, App.Msg>) =
    inherit ReactiveElmishViewModel()

    let filePicker = 
        Program.mkAvaloniaProgram init (update tryPickFile)
        |> Program.withErrorHandler (fun (_, ex) -> printfn "Error: %s" ex.Message)
        |> Program.withConsoleTrace
        |> Program.mkStore

    member this.FilePath = this.Bind (filePicker, _.FilePath >> Option.defaultValue "Not Set")
    member this.Ok() = app.Dispatch (App.SetView App.CounterView)
    member this.PickFile() = filePicker.Dispatch PickFile

    static member DesignVM = 
        let store = Store.design(App.init())
        new FilePickerViewModel(store)
