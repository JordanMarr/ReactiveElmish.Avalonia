namespace AvaloniaXPlatExample.ViewModels

open Elmish.Avalonia

type IStart =
    abstract member StartElmishLoop : view:Avalonia.Controls.Control -> unit

/// Used to bind a view with a view model and provides a method to start the Elmish loop.
type ElmishViewModel<'model, 'msg>(program: AvaloniaProgram<'model, 'msg>) =
    interface IStart with
        member this.StartElmishLoop(view: Avalonia.Controls.Control) =
            try
                program
                |> AvaloniaProgram.startElmishLoop view
            with x ->
                printfn $"StartElmishLoop exited with {x.Message}\n{x.StackTrace}"
