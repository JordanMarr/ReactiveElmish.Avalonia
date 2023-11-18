namespace Elmish.Avalonia

open Elmish
open Avalonia.Controls
open System

module ViewBinder = 

    let bindViewModel (vm: ReactiveUI.IReactiveObject) (view: Control) = 
        view.DataContext <- vm
        vm :?> IDisposable |> Option.ofObj
        |> Option.iter (fun disposable -> 
            view.Unloaded.Add(fun _ -> 
                disposable.Dispose()
            )
        )
