namespace ReactiveElmish

open Elmish
open System.Reactive.Subjects
open System.Reactive.Linq
open System

type IStore<'Model> =
    inherit IDisposable       
    abstract member Model: 'Model with get
    abstract member Observable: IObservable<'Model>

type IStore<'Model, 'Msg> =
    inherit IStore<'Model>
    abstract member Dispatch: 'Msg -> unit

type IHasSubject<'Model> =
    abstract member Subject: Subject<'Model> with get

module Design = 
    /// Stubs a constructor injected dependency in design mode.
    let stub<'T> = Unchecked.defaultof<'T>

/// An Elmish reactive store that can be used to store and update a model and send out an Rx stream of the model.
type ReactiveElmishStore<'Model, 'Msg> () =
    let _modelSubject = new Subject<'Model>()
    let mutable _model: 'Model = Unchecked.defaultof<'Model>
    let mutable _dispatch: 'Msg -> unit = 
        fun _ -> failwith "`Dispatch` failed because the Elmish loop has not been started."

    interface IStore<'Model, 'Msg> with
        member this.Dispatch msg = _dispatch msg
        member this.Model = _model
        member this.Observable = _modelSubject.AsObservable()        
    
    interface IHasSubject<'Model> with
        member this.Subject = _modelSubject
    
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
