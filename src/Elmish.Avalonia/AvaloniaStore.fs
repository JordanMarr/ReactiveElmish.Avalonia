namespace Elmish.Avalonia

open Elmish
open Avalonia.Threading
open Avalonia.Controls
open System.ComponentModel
open System.Reactive.Subjects
open System.Reactive.Linq
open System

type IStore<'Model, 'Msg> =
    inherit IDisposable       
    abstract member Dispatch: 'Msg -> unit
    abstract member Model: 'Model with get
    abstract member Observable: IObservable<'Model>

module Design = 
    /// Stubs a constructor injected dependency in design mode.
    let stub<'T> = Unchecked.defaultof<'T>

type AvaloniaStore<'Model, 'Msg> (program: Program<unit, 'Model, 'Msg, unit>) as this =    
    let _modelSubject = new Subject<'Model>()
    let mutable _model: 'Model = Unchecked.defaultof<'Model>
    let mutable _dispatch: 'Msg -> unit = 
        fun _ -> 
            if not Design.IsDesignMode 
            then failwith "`Dispatch` failed because the Elmish loop has not been started."

    do this.RunProgram(program)

    interface IStore<'Model, 'Msg> with
        member this.Dispatch msg = _dispatch msg
        member this.Model = _model
        member this.Observable = _modelSubject.AsObservable()        
    
    member internal this.Subject = _modelSubject
    
    member private this.RunProgram (program: Elmish.Program<unit, 'Model, 'Msg, unit>) =
        
        // Updates when the Elmish model changes and sends out an Rx stream.
        let setState model (_: Dispatch<'Msg>) =
            _model <- model
            _modelSubject.OnNext(model)

        // A fn that dispatches messages to the Elmish loop.
        let withDispatch (innerDispatch: Dispatch<'Msg>) : Dispatch<'Msg> =
            let dispatch msg = 
                if Dispatcher.UIThread.CheckAccess()
                then innerDispatch msg |> ignore
                else Dispatcher.UIThread.Post(fun () -> innerDispatch msg)

            _dispatch <- dispatch
            dispatch
        
        program
        |> Program.withSetState setState
        |> Program.runWithDispatch withDispatch ()

    interface IDisposable with
        member this.Dispose() =
            _modelSubject.Dispose()
