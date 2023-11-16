namespace Elmish.Avalonia

open Elmish
open Avalonia.Controls
open System

type IStartElmishLoop = 
    abstract member StartElmishLoop : view: Control -> unit

module ViewBinder = 

    let startElmishVM (vm: IStartElmishLoop) (view: Control) = 
        view.DataContext <- vm
        let disposable = vm :?> IDisposable |> Option.ofObj
        disposable
        |> Option.iter (fun disposable -> 
            view.Unloaded.Add(fun _ -> 
                disposable.Dispose()
            )
        )
        vm.StartElmishLoop(view)
