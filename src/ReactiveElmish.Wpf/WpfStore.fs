namespace ReactiveElmish.Wpf

open Elmish
open ReactiveElmish
open System.Windows
open System.Windows.Threading
open System.Threading

type WpfStore<'Model, 'Msg> (program: Program<unit, 'Model, 'Msg, unit>) as this =
    inherit ReactiveElmishStore<'Model, 'Msg>()

    let defaultDispatcher _ = 
        failwith "`Dispatch` failed because the Elmish loop has not been started."

    do this.Dispatcher <- defaultDispatcher
    do this.RunProgram(program)

    /// Creates a WPF dispatcher that can be used to dispatch messages to the Elmish loop.
    override this.WithDispatch (innerDispatch: Dispatch<'Msg>) = 
        let dispatch msg = 
            if Thread.CurrentThread = Application.Current.Dispatcher.Thread
            then innerDispatch msg |> ignore
            else Dispatcher.CurrentDispatcher.Invoke(fun () -> innerDispatch msg)
        
        this.Dispatcher <- dispatch
        dispatch


