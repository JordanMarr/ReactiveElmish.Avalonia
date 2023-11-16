namespace AvaloniaExample.ViewModels

open Elmish.Avalonia
open Elmish
open Messaging
open App

type AboutViewModel(appVM: IReactiveElmishViewModel<Model, Msg>) =
    inherit ReactiveElmishViewModel<Model, Msg>(appVM.Model)

    member this.Version = appVM.Bind _.Version
    member this.Counter = appVM.Bind _.Count
    member this.Ok() = appVM.Dispatch (SetView CounterView)

    override this.StartElmishLoop(view: Avalonia.Controls.Control) = ()

    //static member DesignVM = new AboutViewModel()