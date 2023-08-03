namespace AvaloniaExample.ViewModels

open Elmish.Avalonia

type IElmishViewModel =
    abstract member StartElmishLoop : view: Avalonia.Controls.Control -> unit

/// Used to bind a view with a view model and provides a method to start the Elmish loop.
type ElmishViewModel<'model, 'msg>(program: AvaloniaProgram<'model, 'msg>, ?stopLoopWhenViewIsHidden: bool) =
    let stopLoopWhenViewIsHidden = defaultArg stopLoopWhenViewIsHidden false
    let mutable _view : Avalonia.Controls.Control option = None

    member this.View
        with get() = _view
        and set(value) = _view <- value

    interface IElmishViewModel with
        member this.StartElmishLoop(view: Avalonia.Controls.Control) = 
            this.View <- Some view
            if stopLoopWhenViewIsHidden then
                program
                |> AvaloniaProgram.withTermination 
                    (fun _ -> not view.IsVisible) 
                    (fun _ -> ())
                |> AvaloniaProgram.startElmishLoop view
            else
                program
                |> AvaloniaProgram.startElmishLoop view

