namespace ReactiveElmish

open Elmish
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

type ReactiveElmishStore<'Model, 'Msg> (program: Program<unit, 'Model, 'Msg, unit>) as this =    
    let _modelSubject = new Subject<'Model>()
    let mutable _model: 'Model = Unchecked.defaultof<'Model>
    let mutable _dispatch: 'Msg -> unit = 
        fun _ -> failwith "`Dispatch` failed because the Elmish loop has not been started."

    interface IStore<'Model, 'Msg> with
        member this.Dispatch msg = _dispatch msg
        member this.Model = _model
        member this.Observable = _modelSubject.AsObservable()        
    
    member internal this.Subject = _modelSubject
    
    member this.Dispatcher
        with get() = _dispatch
        and set(dispatch) = _dispatch <- dispatch

    abstract member WithDispatch: Dispatch<'Msg> -> Dispatch<'Msg>
    default this.WithDispatch (innerDispatch: Dispatch<'Msg>) = 
        _dispatch <- innerDispatch
        innerDispatch

    member this.RunProgram (program: Elmish.Program<unit, 'Model, 'Msg, unit>) =
        
        // Updates when the Elmish model changes and sends out an Rx stream.
        let setState model (_: Dispatch<'Msg>) =
            _model <- model
            _modelSubject.OnNext(model)
                    
        program
        |> Program.withSetState setState
        |> Program.runWithDispatch this.WithDispatch ()

    interface IDisposable with
        member this.Dispose() =
            _modelSubject.Dispose()
