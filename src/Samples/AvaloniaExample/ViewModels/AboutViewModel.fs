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

    //override this.StartElmishLoop(view: Avalonia.Controls.Control) = ()

    //static member DesignVM = new AboutViewModel()