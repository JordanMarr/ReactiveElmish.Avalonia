namespace Elmish.Avalonia

open Elmish
open Avalonia.Threading
open Avalonia.Controls
open System.ComponentModel
open System.Reactive.Subjects
open System.Reactive.Linq
open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

type IElmishStore<'Model, 'Msg> =
    inherit IDisposable       
    abstract member Dispatch: 'Msg -> unit
    abstract member Model: 'Model with get
    abstract member ModelObservable: IObservable<'Model>

type DesignStore<'Model, 'Msg>(designModel) = 
    interface IElmishStore<'Model, 'Msg> with
        member this.Dispatch _ = ()
        member this.Model = designModel
        member this.ModelObservable = Observable.Never<'Model>()
    
    interface IDisposable with
        member this.Dispose() = ()

module Store = 
    let design (model: 'Model) = new DesignStore<'Model, 'Msg>(model)


type ElmishStore<'Model, 'Msg> (program: Program<unit, 'Model, 'Msg, unit>) as this =    
    let _modelSubject = new Subject<'Model>()
    let mutable _model: 'Model = Unchecked.defaultof<'Model>
    let mutable _dispatch: 'Msg -> unit = 
        fun _ -> 
            if not Design.IsDesignMode 
            then failwith "`Dispatch` failed because the Elmish loop has not been started."

    do this.RunProgram(program)

    interface IElmishStore<'Model, 'Msg> with
        member this.Dispatch msg = _dispatch msg
        member this.Model = _model
        member this.ModelObservable = _modelSubject.AsObservable()
    
        /// Binds this VM to the view `DataContext` and runs the Elmish loop.
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
