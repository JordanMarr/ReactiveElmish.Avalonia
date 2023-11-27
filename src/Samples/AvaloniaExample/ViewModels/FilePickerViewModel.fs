namespace AvaloniaExample.ViewModels

open ReactiveElmish
open ReactiveElmish.Avalonia
open Elmish
open AvaloniaExample

module FilePicker = 

    type Model = 
        {
            FilePath: string option
        }

    type Msg = 
        | SetFilePath of string option

    let init () = 
        { 
            FilePath = None
        }

    let update (msg: Msg) (model: Model) = 
        match msg with
        | SetFilePath path ->
            { FilePath = path }


open FilePicker

type FilePickerViewModel(fileSvc: FileService) =
    inherit ReactiveElmishViewModel()

    let app = App.app

    let local = 
        Program.mkAvaloniaSimple init update
        |> Program.withErrorHandler (fun (_, ex) -> printfn "Error: %s" ex.Message)
        //|> Program.withConsoleTrace
        |> Program.mkStore

    member this.FilePath = this.Bind (local, _.FilePath >> Option.defaultValue "Not Set")
    member this.Ok() = app.Dispatch App.GoHome
    member this.PickFile() = 
        task {
            let! path = fileSvc.TryPickFile()
            local.Dispatch (SetFilePath path)
        }

    static member DesignVM = new FilePickerViewModel(Design.stub)
