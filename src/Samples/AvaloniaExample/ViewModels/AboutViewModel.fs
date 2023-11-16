namespace AvaloniaExample.ViewModels

open Elmish.Avalonia
open Elmish
open Messaging
open App

type AboutViewModel(app: IElmishStore<Model, Msg>) =
    inherit ReactiveElmishViewModel<Model, Msg>(app.Model)

    member this.Version = app.Bind _.Version
    member this.Counter = app.Bind _.Count
    member this.Ok() = app.Dispatch (SetView CounterView)

    override this.StartElmishLoop(view: Avalonia.Controls.Control) = ()

    //static member DesignVM = new AboutViewModel()