namespace AvaloniaXPlatExample.ViewModels

open Elmish.Avalonia
open Elmish
open App

type AboutViewModel() =
    inherit ReactiveElmishViewModel()

    member this.Version = "v1.0"
    member this.Ok() = app.Dispatch GoHome

    static member DesignVM = 
        new AboutViewModel()