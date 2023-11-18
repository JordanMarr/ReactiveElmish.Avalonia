namespace AvaloniaExample.ViewModels

open Elmish.Avalonia
open Elmish

type AboutViewModel(app: IElmishStore<App.Model, App.Msg>) =
    inherit ReactiveElmishViewModel()

    member this.Version = "v1.0"
    member this.Ok() = app.Dispatch (App.SetView App.CounterView)

    static member DesignVM = 
        let store = Store.design(App.init())
        new AboutViewModel(store)