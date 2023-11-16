namespace AvaloniaExample.ViewModels

open System
open Elmish.Avalonia
open Elmish
open App

type CounterViewModel(appVM: IReactiveElmishViewModel<Model, Msg>) =
    inherit ReactiveUI.ReactiveObject()

    member this.Count = appVM.Bind _.Count
    member this.Actions = appVM.Bind _.Actions
    member this.Increment() = appVM.Dispatch Increment
    member this.Decrement() = appVM.Dispatch Decrement
    member this.Reset() = appVM.Dispatch ResetCounter
    member this.IsResetEnabled = appVM.Bind(fun m -> m.Count <> 0)

    //static member DesignVM = new CounterViewModel(App.init())