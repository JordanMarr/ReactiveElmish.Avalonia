namespace ReactiveElmish.Avalonia

open System.ComponentModel
open ReactiveElmish
open Elmish
open Avalonia.Threading
open Avalonia.Controls

type AvaloniaStore<'Model, 'Msg> (program: Program<unit, 'Model, 'Msg, unit>) as this =
    inherit ReactiveElmishStore<'Model, 'Msg>()

    let defaultDispatcher _ = 
        if not Design.IsDesignMode 
        then failwith "`Dispatch` failed because the Elmish loop has not been started."

    do this.Dispatcher <- defaultDispatcher
    do this.RunProgram(program)

    /// Creates an Avalonia dispatcher that can be used to dispatch messages to the Elmish loop.
    override this.WithDispatch (innerDispatch: Dispatch<'Msg>) = 
        let dispatch msg = 
            if Dispatcher.UIThread.CheckAccess()
            then innerDispatch msg |> ignore
            else Dispatcher.UIThread.Post(fun () -> innerDispatch msg)
        
        this.Dispatcher <- dispatch
        dispatch
