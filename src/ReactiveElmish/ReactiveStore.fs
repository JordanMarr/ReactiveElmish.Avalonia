namespace ReactiveElmish

open System.Reactive.Subjects
open System.Reactive.Linq
open System

/// A reactive store that can be used to store and update a model and send out an Rx stream of the model.
type ReactiveStore<'Model>(init: 'Model) = 
    let _modelSubject = new Subject<'Model>()
    let mutable _model: 'Model = init
    
    interface IStore<'Model> with
        member this.Model = this.Model
        member this.Observable = this.Observable

    member this.Model = _model
    member this.Observable = _modelSubject.AsObservable()

    /// Updates the model and sends out an Rx stream.
    member this.Update(fn: Func<'Model, 'Model>) = 
        _model <- fn.Invoke _model
        _modelSubject.OnNext(_model)

    /// Updates the model and sends out an Rx stream.
    member this.Update(fn: 'Model -> 'Model) = 
        _model <- fn _model
        _modelSubject.OnNext(_model)

    interface IDisposable with
        member this.Dispose() =
            _modelSubject.Dispose()