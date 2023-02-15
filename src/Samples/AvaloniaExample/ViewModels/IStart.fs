namespace AvaloniaExample.ViewModels

open Elmish.Avalonia

type IStart =
    abstract member Start : view:Avalonia.Controls.Control -> unit

/// Used to bind a view with a view model and provides a method to start the Elmish loop.
type Start<'model, 'msg>(program: AvaloniaProgram<'model, 'msg>) =
    interface IStart with
        member this.Start(view: Avalonia.Controls.Control) = 
            program 
            |> AvaloniaProgram.startElmishLoop view
