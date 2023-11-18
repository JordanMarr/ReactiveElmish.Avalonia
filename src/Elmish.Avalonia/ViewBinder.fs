namespace Elmish.Avalonia

open Elmish
open Avalonia.Controls
open System

module ViewBinder = 

    /// Binds a vm to a view and disposes the vm when the view is unloaded.
    let bindWithDispose (vm: ReactiveUI.IReactiveObject) (view: Control) = 
        view.DataContext <- vm
        vm :?> IDisposable |> Option.ofObj
        |> Option.iter (fun disposable -> 
            view.Unloaded.Add(fun _ -> 
                disposable.Dispose()
            )
        )

    /// Binds a vm to a view and does not dispose the vm when the view is unloaded.
    let bindSingleton (vm: ReactiveUI.IReactiveObject) (view: Control) = 
        view.DataContext <- vm
        