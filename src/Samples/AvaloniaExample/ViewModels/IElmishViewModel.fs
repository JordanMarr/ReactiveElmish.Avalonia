namespace AvaloniaExample.ViewModels

open Elmish.Avalonia

type IElmishViewModel =
    abstract member StartElmishLoop : view: Avalonia.Controls.Control -> unit

/// Used to bind a view with a view model and provides a method to start the Elmish loop.
type ElmishViewModel<'model, 'msg>(program: AvaloniaProgram<'model, 'msg>) =
    let mutable _view : Avalonia.Controls.Control option = None

    member val ConfigureView : (Avalonia.Controls.Control -> AvaloniaProgram<'model, 'msg> -> AvaloniaProgram<'model, 'msg>) option = None with get, set

    member this.View
        with get() = _view
        and set(value) = _view <- value

    interface IElmishViewModel with
        member this.StartElmishLoop(view: Avalonia.Controls.Control) = 
            this.View <- Some view
            match this.ConfigureView with
            | Some configureView -> 
                program
                |> configureView view
                |> AvaloniaProgram.startElmishLoop view
            | None ->
                program
                |> AvaloniaProgram.startElmishLoop view

module ElmishViewModel = 
    let create program = ElmishViewModel(program)


    let configureView (fn: Avalonia.Controls.Control -> AvaloniaProgram<'model, 'msg> -> AvaloniaProgram<'model, 'msg>) (vm: ElmishViewModel<'model, 'msg>) = 
        vm.ConfigureView <- Some fn
        vm

    let stopLoopWhenViewIsHidden (vm: ElmishViewModel<'model, 'msg>) = 
        configureView 
            (fun view program -> 
                program
                |> AvaloniaProgram.withTermination 
                    (fun _ -> not view.IsVisible) 
                    (fun _ -> ())
            ) vm

    
    let viewSubscriptions (fn: Avalonia.Controls.Control -> unit) (vm: ElmishViewModel<'model, 'msg>) = 
        configureView
            (fun view program -> 
                program
                |> AvaloniaProgram.withSubscription (fun _ -> fn view)
            ) vm

        
