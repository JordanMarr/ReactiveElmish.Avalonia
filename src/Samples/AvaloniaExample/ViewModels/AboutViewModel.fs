namespace AvaloniaExample.ViewModels

open Elmish.Avalonia
open Elmish
open App

type AboutViewModel(app: IElmishStore<Model, Msg>) =
    inherit ReactiveViewModel()

    member this.Version = this.Bind(app, _.Version)
    member this.Counter = this.Bind(app, _.Count)
    member this.Ok() = app.Dispatch (SetView CounterView)
    member this.ResetCounter() = app.Dispatch Reset

    static member DesignVM = 
        let store = Store.design(App.init())
        new AboutViewModel(store)