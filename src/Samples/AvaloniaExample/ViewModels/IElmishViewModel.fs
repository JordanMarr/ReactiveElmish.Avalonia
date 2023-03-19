namespace AvaloniaExample.ViewModels

open Elmish.Avalonia

type IElmishViewModel =
    abstract member StartElmishLoop : view:Avalonia.Controls.Control -> unit

/// Used to bind a view with a view model and provides a method to start the Elmish loop.
type ElmishViewModel<'model, 'msg>(program: AvaloniaProgram<'model, 'msg>) =
    interface IElmishViewModel with
        member this.StartElmishLoop(view: Avalonia.Controls.Control) = 
            program 
            |> AvaloniaProgram.startElmishLoop view
