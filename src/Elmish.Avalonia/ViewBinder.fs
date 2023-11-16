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

type IOnPropertyChanged = 
    abstract member OnPropertyChanged: [<CallerMemberName; Optional; DefaultParameterValue("")>] ?propertyName: string -> unit

type IRunProgram<'Model, 'Msg> = 
    abstract member RunProgram: Program<unit, 'Model, 'Msg, unit> * Control -> unit

type IStartElmishLoop = 
    abstract member StartElmishLoop : view: Control -> unit

type IElmishStore<'Model, 'Msg> =
    abstract member Dispatch: 'Msg -> unit
    abstract member Model: 'Model with get
    abstract member ModelObservable: IObservable<'Model>

type DesignStore<'Model, 'Msg>(designModel) = 
    interface IElmishStore<'Model, 'Msg> with
        member this.Dispatch _ = ()
        member this.Model = designModel
        member this.ModelObservable = Observable.Never<'Model>()

module ViewBinder = 

    let startElmishVM (vm: IStartElmishLoop) (view: Control) = 
        view.DataContext <- vm
        let disposable = vm :?> IDisposable |> Option.ofObj
        disposable
        |> Option.iter (fun disposable -> 
            view.Unloaded.Add(fun _ -> 
                disposable.Dispose()
            )
        )
        vm.StartElmishLoop(view)
