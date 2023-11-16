namespace AvaloniaExample.ViewModels

open Elmish.Avalonia
open Elmish
open Messaging
open App

type AboutViewModel(app: IElmishStore<Model, Msg>) =
    inherit ReactiveViewModel()

    member this.Version = this.Bind(app, _.Version)
    member this.Counter = this.Bind(app, _.Count)
    member this.Ok() = app.Dispatch (SetView CounterView)

    static member DesignVM = 
        let store = DesignStore<App.Model, App.Msg>(App.init())
        new AboutViewModel(store)