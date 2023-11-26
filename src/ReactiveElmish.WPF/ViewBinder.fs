namespace ReactiveElmish.WPF

open System
open System.Windows.Controls

module ViewBinder = 

    /// Binds a vm to a view's DataContext and disposes the vm when the view is unloaded.
    let bindWithDisposeOnViewUnload (vm: ReactiveUI.IReactiveObject, view: Control) = 
        view.DataContext <- vm
        vm :?> IDisposable |> Option.ofObj
        |> Option.iter (fun disposable -> 
            view.Unloaded.Add(fun _ -> 
                disposable.Dispose()
            )
        )
        vm, view

    /// Binds a vm to a view's DataContext.
    let bind (vm: ReactiveUI.IReactiveObject, view: Control) = 
        view.DataContext <- vm
        vm, view
        